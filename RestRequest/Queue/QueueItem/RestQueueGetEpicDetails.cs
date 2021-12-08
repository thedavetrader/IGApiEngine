using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public static partial class RestQueueQueueItem
    {
        public static void RestQueueGetEpicDetails(List<EpicStreamListItem>? epicStreamListItem)
        {
            if (epicStreamListItem is not null && epicStreamListItem.Any())
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

                string parameters = JsonConvert.SerializeObject(epicStreamListItem, Formatting.None);

                RestRequestQueue restRequestQueueItem = new(nameof(RestRequest.GetEpicDetails), parameters, true, false);

                iGApiDbContext.SaveRestRequestQueue(restRequestQueueItem);

                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
            }
        }
    }
}