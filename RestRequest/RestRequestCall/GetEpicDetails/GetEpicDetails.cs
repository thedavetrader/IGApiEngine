using System;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using Newtonsoft.Json.Linq;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
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

                var response = GetEpicDetailsResponse(epicString);

                #region SyncToDb
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

                if (response is not null)
                {
                    response.Response.marketDetails.ForEach(MarketDetail =>
                    {
                        if (MarketDetail is not null)
                        {
                            var epicDetail = iGApiDbContext.SaveEpicDetail(MarketDetail.instrument);
                        }
                    });

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }
        }
    }
}