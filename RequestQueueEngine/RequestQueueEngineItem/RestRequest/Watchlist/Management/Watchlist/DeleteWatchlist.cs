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

                if (response is not null)
                {
                    if (response.Response.status == "SUCCESS")
                    {
                        WriteLog(Messages($"Watchlist with id \"{request}\" is deleted."));                       
                    }
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
                QueueItemComplete(DeleteWatchlistCompleted);
            }
        }
    }
}