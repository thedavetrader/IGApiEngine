using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? DeleteWatchlistCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void DeleteWatchlist()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                var response = _apiEngine.IGRestApiClient.deleteWatchlist(request).UseManagedCall();

                if (response.Response.status == "SUCCESS")
                {
                    WriteLog(Columns($"Watchlist with id \"{request}\" is deleted."));
                }
                else
                    WriteLog(Columns($"Watchlist request did not result status \"SUCCESS\"."));

                RemoveFromDb(request);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(DeleteWatchlistCompleted);
            }

            void RemoveFromDb(string watchlistId)
            {
                using ApiDbContext apiDbContext = new();

                var removeWatchlist = apiDbContext.Watchlists.Find(_currentAccountId, watchlistId);

                if (removeWatchlist != null)
                {
                    apiDbContext.Watchlists.Remove(removeWatchlist);

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }

        }
    }
}