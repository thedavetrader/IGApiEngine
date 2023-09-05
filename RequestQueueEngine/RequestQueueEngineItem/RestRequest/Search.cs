using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? SearchCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void Search()
        {
            /*
                *  Search markets
                *  - delete search_result (part of integral functioning Search)
                *  - if epic details not yet exists, add
                *  - if result add to search_market_result
                *  - if no result add to search_market_result: [no_result]
                *  - DBProc:
                *      - also delete search_result (to prevent false-positive waitfor)
                *      - request search
                *      - waitfor 
                *          - result
                *      - show result
                * */
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                using ApiDbContext apiDbContext = new();

                apiDbContext.SearchResults.RemoveRange(apiDbContext.SearchResults);

                var response = _apiEngine.IGRestApiClient.searchMarket(request).UseManagedCall();

                var markets = response.Response.markets;
                var newEpics = markets.Where(w => !apiDbContext.EpicDetails.Any(a => a.Epic == w.epic));

                if (markets.Any())
                {
                    // First retreive epic detail if not yet exists on db, preventing FK errors inserting WatchlistEpicDetails.
                    if (newEpics is not null && newEpics.Any())
                    {
                        var parameters = newEpics.Select(s => new { s.epic }).Distinct();

                        if (parameters.Any())
                        {
                            string? jsonParameters = null;

                            jsonParameters = JsonConvert.SerializeObject(
                                parameters,
                                Formatting.None);

                            var getEpicDetailsGuid = Guid.NewGuid();

                            // Using eventhandler to queue this when done. Otherwise it would fire almost immediately, while the epic details are still beeing retreived.
                            void GetEpicDetailsCompleted(object? sender, EventArgs e)
                            {
                                if (sender is not null)
                                {
                                    var senderEngineItem = (RequestQueueEngineItem)sender;

                                    if (senderEngineItem.ApiRequestQueueItem.Guid == getEpicDetailsGuid)
                                    {
                                        using ApiDbContext apiDbContext = new();

                                        apiDbContext.SearchResults.AddRange(markets.Select(m => new SearchResult(m.epic)));

                                        Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

                                        RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;
                                    }
                                }
                            }

                            RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;    // Prevent event is subscribed more then once.
                            RequestQueueEngineItem.GetEpicDetailsCompleted += GetEpicDetailsCompleted;

                            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetEpicDetails), true, false, getEpicDetailsGuid, null, jsonParameters, cancellationToken: _cancellationToken);
                        }
                    }
                    else
                    {
                        apiDbContext.SearchResults.AddRange(markets.Select(m => new SearchResult(m.epic)));

                        Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                    }
                }
                else
                {
                    apiDbContext.SearchResults.Add(new SearchResult("[NO_RESULTS_FOUND]"));
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
                QueueItemComplete(SearchCompleted);
            }
        }
    }
}