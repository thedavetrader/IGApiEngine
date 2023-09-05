using dto.endpoint.clientsentiment;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.Model;
using IGWebApiClient;
using Newtonsoft.Json.Linq;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetClientSentimentCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetClientSentiment()
        {
            const int splitSize = 500;

            try
            {
                string marketIdString;
                List<String>? requestedMarketIds = null;

                if (ApiRequestQueueItem.Parameters is not null)
                {
                    var request = ApiRequestQueueItem.Parameters.Replace(" ", string.Empty); // for robustness sake trim spaces. Otherwise a value preceded by space does not equals value without space in Linq queries.
                    requestedMarketIds = request.Split(',').ToList();
                }

                using ApiDbContext apiDbContext = new();

                var marketIds = apiDbContext.EpicDetails
                    .Where(w => !string.IsNullOrEmpty(w.MarketId)).ToList()
                    .Where(t => requestedMarketIds is null || requestedMarketIds.Any(marketId => marketId == t.MarketId))
                    .Select(s => s.MarketId)
                    .Distinct()
                    .ToList();

                var split = marketIds.Count > splitSize;

                int range = split ? splitSize : marketIds.Count;

                marketIdString = String.Join(",", marketIds.GetRange(0, range));

                if (!string.IsNullOrEmpty(marketIdString))
                {
                    var response = _apiEngine.IGRestApiClient.getClientSentimentMulti(marketIdString).UseManagedCall();

                    SyncToDbClientSentiment(response);
                }

                marketIds.RemoveRange(0, range);

                //  Split remaining
                if (split)
                {
                    foreach (var marketIdSplitList in marketIds.Split(splitSize))
                    {
                        marketIdString = String.Join(",", marketIdSplitList);

                        RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetClientSentiment), executeAsap: false, isRecurrent: false, guid: Guid.NewGuid(), parentGuid: ApiRequestQueueItem.Guid, parameters: marketIdString, cancellationToken: _cancellationToken);
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
                QueueItemComplete(GetClientSentimentCompleted);
            }

            void SyncToDbClientSentiment(IgResponse<ClientSentimentList> response)
            {
                if (response.Response is not null)
                {
                    ApiDbContext apiDbContext = new();

                    response.Response.clientSentiments.ForEach(clientSentiment =>
                    {
                        if (clientSentiment is not null)
                            apiDbContext.SaveClientSentiment(clientSentiment);
                    });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}