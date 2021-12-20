using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? CreateWatchlistCompleted;
        public void CreateWatchlist(string watchlistRequest)
        {
            try
            {
                dto.endpoint.watchlists.manage.create.CreateWatchlistRequest createWatchlistRequest = JsonConvert.DeserializeObject<dto.endpoint.watchlists.manage.create.CreateWatchlistRequest>(watchlistRequest);

                var response = _apiEngine.IGRestApiClient.createWatchlist(createWatchlistRequest).UseManagedCall();

                if (response is not null)
                {
                    if (response.Response.status == "SUCCESS")
                    {
                        WriteLog(Messages($"Watchlist with id \"{response.Response.watchlistId}\" is created."));
                    }
                    else if (response.Response.status == "SUCCESS_NOT_ALL_INSTRUMENTS_ADDED")
                    {
                        WriteLog(Messages($"Watchlist with id \"{response.Response.watchlistId}\" is created, but not all instruments were added."));
                    }
                    else
                        WriteLog(Messages($"Watchlist request did not result status \"SUCCESS\"."));

                    //TODO: Queue get watchlistinstruments

                }
                else
                    throw new RestCallNullReferenceException();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                CreateWatchlistCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}