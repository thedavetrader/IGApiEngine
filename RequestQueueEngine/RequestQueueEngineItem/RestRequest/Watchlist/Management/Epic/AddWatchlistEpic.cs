using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? AddWatchlistEpicCompleted;
        private class CustomAddWatchlistEpicRequest
        {
            public string? WatchlistId { get; set; }

            public dto.endpoint.watchlists.manage.edit.AddInstrumentToWatchlistRequest? AddInstrumentToWatchlistRequest { get; set; }
        }

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void AddWatchlistEpic()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomAddWatchlistEpicRequest addWatchlistEpicRequest = JsonConvert.DeserializeObject<CustomAddWatchlistEpicRequest>(request);

                if (!string.IsNullOrEmpty(addWatchlistEpicRequest.WatchlistId) && addWatchlistEpicRequest.AddInstrumentToWatchlistRequest is not null)
                {
                    var response = _apiEngine.IGRestApiClient.addInstrumentToWatchlist(addWatchlistEpicRequest.WatchlistId, addWatchlistEpicRequest.AddInstrumentToWatchlistRequest).UseManagedCall();

                    using ApiDbContext apiDbContext = new();
                    apiDbContext.SaveWatchlistEpicDetail(_currentAccountId, addWatchlistEpicRequest.WatchlistId, addWatchlistEpicRequest.AddInstrumentToWatchlistRequest.epic);
                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(AddWatchlistEpicCompleted);
            }
        }
    }
}