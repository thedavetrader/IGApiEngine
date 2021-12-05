using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json.Linq;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        private void GetEpicDetails(string epics)
        {
            var jsonEpicArray = JArray.Parse(epics);
            var epicList = new List<string>();
            string epicString;

            foreach (JObject item in jsonEpicArray)
            {
                //TODO: check if limit of 50 is reached. (Theoretically not likely)
                if (item is not null && item.GetValue("epic") is not null)
                    epicList.Add(item.GetValue("epic").ToString());
            }

            epicString = String.Join(",", epicList);

            using IGApiDbContext iGApiDbContext = new();

            _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

            #region RestRequestCall
            var response = Task.Run(async () => await _iGApiEngine.IGRestApiClient.marketDetailsMulti(epicString)).Result;

            _ = response ?? throw new RestCallNullReferenceException(nameof(response));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)    // TODO Retry on forbidden?
                throw new RestCallHttpRequestException(nameof(GetEpicDetails), response.StatusCode);
            #endregion RestRequestCall

            response.Response.marketDetails.ForEach(MarketDetail =>
            {
                if (MarketDetail is not null)
                {
                    iGApiDbContext.SaveEpicDetail(MarketDetail.instrument);
                }
            });

            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
        }
    }
}