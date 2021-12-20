using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? DeleteWatchlistCompleted;
        public void DeleteWatchlist(string watchlistId)
        {
            try
            {
                var response = _apiEngine.IGRestApiClient.deleteWatchlist(watchlistId).UseManagedCall();

                if (response is not null)
                {
                    if (response.Response.status == "SUCCESS")
                        WriteLog(Messages($"Watchlist with id \"{watchlistId}\" is deleted."));
                    else
                        WriteLog(Messages($"Watchlist request did not result status \"SUCCESS\"."));
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
                DeleteWatchlistCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}