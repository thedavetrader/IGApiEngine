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

                if (response.Response.status == "SUCCESS" || response.Response.status == "SUCCESS_NOT_ALL_INSTRUMENTS_ADDED")
                {
                    if (response.Response.status == "SUCCESS")
                    {

                        WriteLog(Columns($"Watchlist with id \"{response.Response.watchlistId}\" is created."));
                    }
                    else if (response.Response.status == "SUCCESS_NOT_ALL_INSTRUMENTS_ADDED")
                    {
                        WriteLog(Columns($"Watchlist with id \"{response.Response.watchlistId}\" is created, but not all instruments were added."));
                    }

                    SaveToDb(createWatchlistRequest, response);

                }
                else
                    WriteLog(Columns($"Watchlist request did not result status \"SUCCESS\"."));
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

            void SaveToDb(dto.endpoint.watchlists.manage.create.CreateWatchlistRequest createWatchlistRequest, IGWebApiClient.IgResponse<dto.endpoint.watchlists.manage.create.CreateWatchlistResponse> response)
            {
                using ApiDbContext apiDbContext = new();

                apiDbContext.Watchlists.Add(new Watchlist(
                                            new dto.endpoint.watchlists.retrieve.Watchlist()
                                            {
                                                id = response.Response.watchlistId,
                                                name = createWatchlistRequest.name,
                                                editable = true,
                                                deleteable = true,
                                                defaultSystemWatchlist = false
                                            },
                                            _currentAccountId));

                Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
            }

        }
    }
}