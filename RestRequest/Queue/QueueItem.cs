using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public static partial class QueueQueueItem
    {
        public static void QueueItem(
            string restRequest, 
            [NotNullAttribute] bool executeAsap,
            [NotNullAttribute] bool isRecurrent,
            string? parameters = null)
        {
            using IGApiDbContext iGApiDbContext = new();

            _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

            RestRequestQueueItem restRequestQueueItem = new(restRequest, parameters, executeAsap, isRecurrent);

            iGApiDbContext.SaveRestRequestQueue(restRequestQueueItem);

            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
        }
    }
}