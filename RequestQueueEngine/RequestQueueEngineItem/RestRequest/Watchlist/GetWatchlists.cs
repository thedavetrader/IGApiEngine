using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Configuration;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetWatchlistsCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetWatchlists()
        {
            try
            {
                using ApiDbContext apiDbContext = new();

                var response = _apiEngine.IGRestApiClient.listOfWatchlists().UseManagedCall();

                if (response.Response is not null)
                {
                    response.Response.watchlists.ForEach(watchlist =>
                    {
                        if (watchlist is not null)
                            apiDbContext.SaveWatchlist(watchlist, _currentAccountId);
                    });

                    // Remove obsolete
                    apiDbContext.RemoveRange(apiDbContext.Watchlists
                        .Where(w => w.AccountId == _currentAccountId).ToList()
                        .Where(y => !response.Response.watchlists.Any(a => a.id == y.WatchlistId))
                        );

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
                QueueItemComplete(GetWatchlistsCompleted);
            }
        }
    }
}