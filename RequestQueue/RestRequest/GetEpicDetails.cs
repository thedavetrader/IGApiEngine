using System;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using Newtonsoft.Json.Linq;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? GetEpicDetailsCompleted;
        //TODO: GetEpicDetails Make version that refresh epics in epicstreamlist UNION existing epic details on db.
        //      Use restapiqueue to queue the request and split over max limit 50.
        [Obsolete("Make version that recurrently refresh epics in epicstreamlist UNION existing epic details on db + all watchlists. Use restapiqueue to queue the request and split over max limit 50.")]
        public void GetEpicDetails(string epics)
        {
            try
            {
                #region Parse parameters
                var jsonEpicArray = JArray.Parse(epics);
                var epicList = new List<string>();
                string epicString;

                if (jsonEpicArray.Count > 40)
                    throw new Exception("[CRITICAL_ERROR] IG does not allow more then 40 concurrent streaming subscriptions.");

                foreach (JObject item in jsonEpicArray)
                {
                    var addItem = item.GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>();

                    if (item is not null && addItem is not null)
                        epicList.Add(addItem);
                }

                epicString = String.Join(",", epicList);
                #endregion

                var response = _apiEngine.IGRestApiClient.marketDetailsMulti(epicString).UseManagedCall();

                SyncToDbEpicDetails(response);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                GetEpicDetailsCompleted?.Invoke(this, EventArgs.Empty);
            }

            static void SyncToDbEpicDetails(IgResponse<MarketDetailsListResponse>? response)
            {
                IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

                if (response is not null)
                {
                    response.Response.marketDetails.ForEach(MarketDetail =>
                    {
                        if (MarketDetail is not null)
                            iGApiDbContext.SaveEpicDetail(MarketDetail.instrument, MarketDetail.dealingRules);
                    });

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}