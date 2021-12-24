using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? AddWatchlistEpicCompleted;
        private class CustomAddWatchlistEpicRequest
        {
            public string? watchlistId { get; set; }

            public dto.endpoint.watchlists.manage.edit.AddInstrumentToWatchlistRequest? AddInstrumentToWatchlistRequest { get; set; }
        }

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void AddWatchlistEpic()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomAddWatchlistEpicRequest addWatchlistEpicRequest = JsonConvert.DeserializeObject<CustomAddWatchlistEpicRequest>(request);

                if (!string.IsNullOrEmpty(addWatchlistEpicRequest.watchlistId) && addWatchlistEpicRequest.AddInstrumentToWatchlistRequest is not null) 
                {
                    var response = _apiEngine.IGRestApiClient.addInstrumentToWatchlist(addWatchlistEpicRequest.watchlistId, addWatchlistEpicRequest.AddInstrumentToWatchlistRequest).UseManagedCall();

                    if (response is not null)
                    {
                        using ApiDbContext apiDbContext = new();
                        apiDbContext.SaveWatchlistEpicDetail(_currentAccountId, addWatchlistEpicRequest.watchlistId, addWatchlistEpicRequest.AddInstrumentToWatchlistRequest.epic);
                        Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
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
                QueueItemComplete(AddWatchlistEpicCompleted);
            }
        }
    }
}