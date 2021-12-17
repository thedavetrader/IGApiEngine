using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public static void QueueItem(
            string restRequest,
            [NotNull] bool executeAsap,
            [NotNull] bool isRecurrent,
            string? parameters = null)
        {
            using IGApiDbContext iGApiDbContext = new();

            _ = iGApiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ApiRequestQueueItems));

            ApiRequestQueueItem restRequestQueueItem = new(restRequest, parameters, executeAsap, isRecurrent);

            iGApiDbContext.SaveRestRequestQueue(restRequestQueueItem);

            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
        }
    }
}