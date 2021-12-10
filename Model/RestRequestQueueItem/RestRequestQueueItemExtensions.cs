using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static RestRequestQueueItem? SaveRestRequestQueue(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] RestRequestQueueItem restRequestQueueItem
            )
        {
            _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

            var existingrestRequestQueueItem =
                    Task.Run(() => iGApiDbContext.RestRequestQueue
                        .FirstOrDefault(w => 
                            w.IsRecurrent && w.RestRequest == restRequestQueueItem.RestRequest ||
                            !w.IsRecurrent &&  w.ExecuteAsap == restRequestQueueItem.ExecuteAsap &&  w.RestRequest == restRequestQueueItem.RestRequest
                            )).Result 
                    ;

            if (existingrestRequestQueueItem is not null)
                existingrestRequestQueueItem.MapProperties(restRequestQueueItem);
            else
                existingrestRequestQueueItem = iGApiDbContext.RestRequestQueue.Add(restRequestQueueItem).Entity;

            return existingrestRequestQueueItem;
        }
    }

    //public static void UpdateProperties(this DbContext context, object target, object source)
    //{
    //    foreach (var propertyEntry in context.Entry(target).Properties)
    //    {
    //        var property = propertyEntry.Metadata;
    //        // Skip shadow and key properties
    //        if (property.IsShadowProperty() || (propertyEntry.EntityEntry.IsKeySet && property.IsKey())) continue;
    //        propertyEntry.CurrentValue = property.GetGetter().GetClrValue(source);
    //    }
    //}
}