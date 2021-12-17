using dto.endpoint.clientsentiment;
using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using Newtonsoft.Json.Linq;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public void GetClientSentiment()
        {
            try
            {
                string marketIdString;

                using (IGApiDbContext iGApiDbContext = new())
                {
                    _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

                    marketIdString = String.Join(",", iGApiDbContext.EpicDetails.Select(s => s.MarketId));
                }

                if (!string.IsNullOrEmpty(marketIdString))
                {
                    var response = _apiEngine.IGRestApiClient.getClientSentimentMulti(marketIdString).UseManagedCall();

                    SyncToDbClientSentiment(response);
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }

            static void SyncToDbClientSentiment(IgResponse<ClientSentimentList>? response)
            {
                IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.ClientSentiments ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ClientSentiments));

                if (response is not null)
                {
                    response.Response.clientSentiments.ForEach(clientSentiment =>
                    {
                        if (clientSentiment is not null)
                            iGApiDbContext.SaveClientSentiment(clientSentiment);
                    });

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}