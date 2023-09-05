using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.Model;
using Microsoft.EntityFrameworkCore;

namespace IGApi.RequestQueue
{
    using static Log;

    public partial class RequestQueueEngineItem
    {
        public ApiRequestQueueItem ApiRequestQueueItem;

        private readonly Task? _requestCallTask;

        private readonly ApiEngine _apiEngine = ApiEngine.Instance;

        private readonly string _currentAccountId;

        private readonly DateTime _dequeueTimestamp;

        private readonly CancellationToken _cancellationToken;

        public RequestQueueEngineItem(
            [NotNullAttribute] ApiRequestQueueItem apiRequestQueueItem,
            [NotNullAttribute] DateTime dequeueTimestamp,
            [NotNullAttribute] CancellationToken cancellationToken
            )
        {
            _cancellationToken = cancellationToken;

            if (!_apiEngine.IsLoggedIn && !_cancellationToken.IsCancellationRequested) throw new IGApiConncectionError();

            ApiRequestQueueItem = apiRequestQueueItem;

            _dequeueTimestamp = dequeueTimestamp;

            _currentAccountId = _apiEngine.LoginSessionInformation.currentAccountId;


            var request = this.GetType().GetMethod(ApiRequestQueueItem.Request);

            if (request is not null)
            {
                RequestTypeAttribute? requestTypeAttribute = (RequestTypeAttribute?)Attribute.GetCustomAttribute(request, typeof(RequestTypeAttribute));

                if (requestTypeAttribute is not null)
                {
                    _requestCallTask = new Task(() => request.Invoke(this, Array.Empty<object>()));
                }
                else
                    throw new Exception($"Could not find RequestTypeAttribute of request {ApiRequestQueueItem.Request}");
            }
            else
                throw new Exception($"The request \"{ApiRequestQueueItem.Request}\" is not implemented.");
        }

        public void Execute()
        {
            if (_requestCallTask is not null)
                _requestCallTask.FireAndForget();
            else
                WriteLog(Columns($"Execution of the rest request {ApiRequestQueueItem.Request} is not implemented yet."));
        }

        private void RemoveObsoleteEpicTicks(ApiDbContext apiDbContext)
        {
            var obsoleteEpicTicks = apiDbContext.EpicTicks
                    .ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                    .Where(w => !ApiEngine.EpicStreamPriceAvailableCheck(w.Epic));

            //  Remoe EpicTicks of which streaming prices are not available. These are leftovers from when a position was opened, despite that 
            //  at that time it could not be determined wether streamprices were available for that the particular epic of that position.
            apiDbContext.EpicTicks.RemoveRange(obsoleteEpicTicks);

            //  Remove from list
            _apiEngine.RemoveEpicStreamListItems(_apiEngine.EpicStreamList.ToList().Where(f => obsoleteEpicTicks.Any(s => s.Epic == f.Epic)));
        }

        private void QueueItemComplete(EventHandler? eventHandler)
        {
            // Formerly used for deleting request item from apirequest queue.
            // Now obsolete, but method pattern is kept for future functionality/events.
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        public static void QueueItem(
            string restRequest,
            [NotNull] bool executeAsap,
            [NotNull] bool isRecurrent,
            Guid guid,
            Guid? parentGuid = null,
            string? parameters = null,
            CancellationToken cancellationToken = default
            )
        {
            using ApiDbContext apiDbContext = new();

            if (!apiDbContext.ApiRequestQueueItems.Any(a => a.Guid == guid))
            {
                ApiRequestQueueItem apiRequestQueueItem = new(
                    restRequest: restRequest,
                    parameters: parameters,
                    executeAsap: executeAsap,
                    isRecurrent: isRecurrent,
                    guid: guid,
                    parentGuid: parentGuid);

                apiRequestQueueItem.SaveApiRequestQueueItem(apiDbContext.ConnectionString);
            }
        }
    }
}
