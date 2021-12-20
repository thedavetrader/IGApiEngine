using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? GetWatchlistsCompleted;
        //TODO: Make functionality that can create auto watchlists based on signals (let's begin with trending i.c.w. clientsentiment)
        public void GetWatchlists()
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.Watchlists ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Watchlists));

                var response = _apiEngine.IGRestApiClient.listOfWatchlists().UseManagedCall();

                if (response is not null)
                {
                    response.Response.watchlists.ForEach(watchlist =>
                    {
                        if (watchlist is not null)
                            iGApiDbContext.SaveWatchlist(watchlist, _currentAccountId);
                    });

                    // Remove obsolete
                    iGApiDbContext.RemoveRange(iGApiDbContext.Watchlists
                        .Where(w => w.AccountId == _currentAccountId).ToList()
                        .Where(y => !response.Response.watchlists.Any(a => a.id == y.Id))
                        );

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

                    // Create default watchlist if not found
                    //  TODO: Make configurable name of default watchlist
                    var defaultWatchListName = "Signals";

                    if (!iGApiDbContext.Watchlists.Any(a => a.Name == defaultWatchListName))
                    {
                        string? jsonParameters = null;
                        jsonParameters = JsonConvert.SerializeObject(new { name = defaultWatchListName }, Formatting.None);

                        RequestQueueItem.QueueItem(nameof(RequestQueueItem.CreateWatchlist), true, false, jsonParameters);
                        RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetWatchlists), true, false);
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
                GetWatchlistsCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}