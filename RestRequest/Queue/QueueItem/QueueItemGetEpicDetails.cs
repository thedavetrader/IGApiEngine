using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public static partial class QueueQueueItem
    {
        public static void QueueItemGetEpicDetails(List<string>? epics)
        {
            if (epics is not null && epics.Any())
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

                string parameters = JsonConvert.SerializeObject(
                    epics.Select(epic => new { epic }).Distinct(),
                    Formatting.None);

                RestRequestQueue restRequestQueueItem = new(nameof(RestRequest.GetEpicDetails), parameters, true, false);

                iGApiDbContext.SaveRestRequestQueue(restRequestQueueItem);

                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
            }
        }
    }
}