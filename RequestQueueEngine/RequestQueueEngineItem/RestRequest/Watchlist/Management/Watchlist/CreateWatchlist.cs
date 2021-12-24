using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? CreateWatchlistCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void CreateWatchlist()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                dto.endpoint.watchlists.manage.create.CreateWatchlistRequest createWatchlistRequest = JsonConvert.DeserializeObject<dto.endpoint.watchlists.manage.create.CreateWatchlistRequest>(request);

                var response = _apiEngine.IGRestApiClient.createWatchlist(createWatchlistRequest).UseManagedCall();

                if (response is not null)
                {
                    if (response.Response.status == "SUCCESS" || response.Response.status == "SUCCESS_NOT_ALL_INSTRUMENTS_ADDED")
                    {
                        if (response.Response.status == "SUCCESS")
                        {
                            WriteLog(Messages($"Watchlist with id \"{response.Response.watchlistId}\" is created."));
                        }
                        else if (response.Response.status == "SUCCESS_NOT_ALL_INSTRUMENTS_ADDED")
                        {
                            WriteLog(Messages($"Watchlist with id \"{response.Response.watchlistId}\" is created, but not all instruments were added."));
                        }
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
                QueueItemComplete(CreateWatchlistCompleted);
            }
        }
    }
}