using dto.endpoint.clientsentiment;
using IGApi.Common;
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
            try
            {
                string marketIdString;

                using (ApiDbContext apiDbContext = new())
                {
                    _ = apiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetails));

                    marketIdString = String.Join(",", apiDbContext.EpicDetails.Where(w => !string.IsNullOrEmpty(w.MarketId)).Select(s => s.MarketId).Distinct());
                }

                if (!string.IsNullOrEmpty(marketIdString))
                {
                    var response = _apiEngine.IGRestApiClient.getClientSentimentMulti(marketIdString).UseManagedCall();

                    SyncToDbClientSentiment(response);
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

            static void SyncToDbClientSentiment(IgResponse<ClientSentimentList>? response)
            {
                ApiDbContext apiDbContext = new();

                _ = apiDbContext.ClientSentiments ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ClientSentiments));

                if (response is not null)
                {
                    response.Response.clientSentiments.ForEach(clientSentiment =>
                    {
                        if (clientSentiment is not null)
                            apiDbContext.SaveClientSentiment(clientSentiment);
                    });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}