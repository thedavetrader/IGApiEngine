using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.Model;

namespace IGApi.RequestQueue
{
    using static Log;

    public partial class RequestQueueEngineItem
    {
        public bool IsTradingRequest;

        public bool IsRestRequest;

        public ApiRequestQueueItem ApiRequestQueueItem;

        private readonly Task? _restRequestCallTask;

        private readonly ApiEngine _apiEngine = ApiEngine.Instance;

        private readonly string _currentAccountId;

        private DateTime _dequeueTimestamp;

        public RequestQueueEngineItem(
            [NotNullAttribute] ApiRequestQueueItem apiRequestQueueItem,
            [NotNullAttribute] DateTime dequeueTimestamp
            )
        {
            if (!_apiEngine.IsLoggedIn) throw new IGApiConncectionError();

            ApiRequestQueueItem = apiRequestQueueItem;
            
            _dequeueTimestamp = dequeueTimestamp;

            _currentAccountId = _apiEngine.LoginSessionInformation.currentAccountId;

            var request = this.GetType().GetMethod(ApiRequestQueueItem.Request);

            if (request is not null)
            {
                RequestTypeAttribute? requestTypeAttribute = (RequestTypeAttribute?)Attribute.GetCustomAttribute(request, typeof(RequestTypeAttribute));

                if (requestTypeAttribute is not null)
                {
                    IsRestRequest = requestTypeAttribute.IsRestRequest;

                    IsTradingRequest = requestTypeAttribute.IsTradingRequest;

                    _restRequestCallTask = new Task(() => request.Invoke(this, new object[] { }));
                }
                else
                    throw new Exception($"Could not find RequestTypeAttribute of request {ApiRequestQueueItem.Request}");
            }
            else
                throw new Exception($"The request \"{ApiRequestQueueItem.Request}\" is not implemented.");
        }

        public void Execute()
        {
            if (_restRequestCallTask is not null)
                _restRequestCallTask.FireAndForget();
            else
                WriteLog(Messages($"Execution of the rest request {ApiRequestQueueItem.Request} is not implemented yet."));
        }

        private void RemoveObsoleteEpicTicks(ApiDbContext apiDbContext)
        {
            _ = apiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicTicks));

            var obsoleteEpicTicks = apiDbContext.EpicTicks
                    .ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                    .Where(w => !ApiEngine.EpicStreamPriceAvailableCheck(w.Epic));

            //  Remoe EpicTicks of which streaming prices are not available. These are leftovers from when a position was opened, despite that 
            //  at that time it could not be determined wether streamprices were available for that the particular epic of that position.
            apiDbContext.EpicTicks.RemoveRange(obsoleteEpicTicks);

            //  Remove from list
            _apiEngine.RemoveEpicStreamListItems(_apiEngine.EpicStreamList.Where(f => obsoleteEpicTicks.Any(s => s.Epic == f.Epic)));
        }

        private void QueueItemComplete(EventHandler? eventHandler)
        {
            using ApiDbContext apiDbContext = new();

            _ = apiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiRequestQueueItems));

            apiDbContext.ApiRequestQueueItems.Attach(ApiRequestQueueItem);

            if (ApiRequestQueueItem.IsRecurrent && !IsTradingRequest && !ApiRequestQueueItem.ExecuteAsap)
            {
                ApiRequestQueueItem.Timestamp = _dequeueTimestamp;
                ApiRequestQueueItem.IsRunning = false;
            }
            else
                apiDbContext.ApiRequestQueueItems.Remove(ApiRequestQueueItem);

            Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        public static void QueueItem(
            string restRequest,
            [NotNull] bool executeAsap,
            [NotNull] bool isRecurrent,
            Guid guid,
            Guid? parentGuid = null,
            string? parameters = null)
        {
            using ApiDbContext apiDbContext = new();

            _ = apiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiRequestQueueItems));

            ApiRequestQueueItem apiRequestQueueItem = new(
                restRequest: restRequest, 
                parameters: parameters,
                executeAsap: executeAsap,
                isRecurrent: isRecurrent,
                guid: guid,
                parentGuid: parentGuid);

            apiDbContext.SaveRestRequestQueue(apiRequestQueueItem);

            Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();
        }
    }
}
