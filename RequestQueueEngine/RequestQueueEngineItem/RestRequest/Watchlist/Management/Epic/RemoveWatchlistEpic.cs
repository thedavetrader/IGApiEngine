using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? RemoveWatchlistEpicCompleted;
        private class CustomRemoveWatchlistEpicRequest
        {
            public string? watchlistId { get; set; }
            public string? epic { get; set; }
        }

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void RemoveWatchlistEpic()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomRemoveWatchlistEpicRequest removeWatchlistEpicRequest = JsonConvert.DeserializeObject<CustomRemoveWatchlistEpicRequest>(request);

                if (!string.IsNullOrEmpty(removeWatchlistEpicRequest.watchlistId) && !string.IsNullOrEmpty(removeWatchlistEpicRequest.epic))
                {
                    var response = _apiEngine.IGRestApiClient.removeInstrumentFromWatchlist(removeWatchlistEpicRequest.watchlistId, removeWatchlistEpicRequest.epic).UseManagedCall();

                    if (response is not null)
                    {
                        using ApiDbContext apiDbContext = new();
                        _ = apiDbContext.WatchlistEpicDetails ?? throw new DBContextNullReferenceException(nameof(apiDbContext.WatchlistEpicDetails));

                        var removeWatchlistEpic = apiDbContext.WatchlistEpicDetails.Find(_currentAccountId, removeWatchlistEpicRequest.watchlistId, removeWatchlistEpicRequest.epic);

                        if (removeWatchlistEpic is not null)
                        {
                            apiDbContext.Remove(removeWatchlistEpic);
                            Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                        }
                    }
                    else
                        throw new RestCallNullReferenceException();

                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(RemoveWatchlistEpicCompleted);
            }
        }
    }
}