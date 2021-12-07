using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public static partial class RestQueueQueueItem
    {
        public static void RestQueueGetOpenPositions()
        {
            var restRequest = nameof(RestRequest.GetOpenPositions);

            using IGApiDbContext iGApiDbContext = new();

            _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

            if (!iGApiDbContext.RestRequestQueue.Where(w => w.RestRequest == restRequest && w.IsRecurrent).Any())
                iGApiDbContext.SaveRestRequestQueue(new(restRequest, null, false, true));

            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
        }
    }
}