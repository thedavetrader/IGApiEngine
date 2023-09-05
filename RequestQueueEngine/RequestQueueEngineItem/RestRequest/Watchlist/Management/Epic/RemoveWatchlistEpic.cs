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
            public string? WatchlistId { get; set; }
            public string? Epic { get; set; }
        }

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void RemoveWatchlistEpic()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomRemoveWatchlistEpicRequest removeWatchlistEpicRequest = JsonConvert.DeserializeObject<CustomRemoveWatchlistEpicRequest>(request);

                if (!string.IsNullOrEmpty(removeWatchlistEpicRequest.WatchlistId) && !string.IsNullOrEmpty(removeWatchlistEpicRequest.Epic))
                {
                    var response = _apiEngine.IGRestApiClient.removeInstrumentFromWatchlist(removeWatchlistEpicRequest.WatchlistId, removeWatchlistEpicRequest.Epic).UseManagedCall();

                        using ApiDbContext apiDbContext = new();

                        var removeWatchlistEpic = apiDbContext.WatchlistEpicDetails.Find(_currentAccountId, removeWatchlistEpicRequest.WatchlistId, removeWatchlistEpicRequest.Epic);

                        if (removeWatchlistEpic is not null)
                        {
                            apiDbContext.Remove(removeWatchlistEpic);
                            Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                        }
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