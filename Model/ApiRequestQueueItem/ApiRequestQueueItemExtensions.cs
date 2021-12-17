using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ApiRequestQueueItem? SaveRestRequestQueue(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] ApiRequestQueueItem apiRequestQueueItem
            )
        {
            _ = iGApiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ApiRequestQueueItems));

            var currentRestRequestQueueItem =
                    Task.Run(() => iGApiDbContext.ApiRequestQueueItems
                        .FirstOrDefault(w => 
                            w.IsRecurrent && w.RestRequest == apiRequestQueueItem.RestRequest ||
                            !w.IsRecurrent &&  w.ExecuteAsap == apiRequestQueueItem.ExecuteAsap &&  w.RestRequest == apiRequestQueueItem.RestRequest
                            )).Result 
                    ;

            if (currentRestRequestQueueItem is not null)
                currentRestRequestQueueItem.MapProperties(apiRequestQueueItem);
            else
                currentRestRequestQueueItem = iGApiDbContext.ApiRequestQueueItems.Add(apiRequestQueueItem).Entity;

            return currentRestRequestQueueItem;
        }
    }

    //TODO:     ZEROPRIO Example update properties
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