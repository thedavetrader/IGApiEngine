using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ApiRequestQueueItem? SaveRestRequestQueue(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] ApiRequestQueueItem apiRequestQueueItem
            )
        {
            _ = apiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiRequestQueueItems));

            var currentRestRequestQueueItem =
                    Task.Run(() => apiDbContext.ApiRequestQueueItems
                        .FirstOrDefault(w => 
                        w.Guid == apiRequestQueueItem.Guid
                            )).Result 
                    ;

            if (currentRestRequestQueueItem is not null)
                currentRestRequestQueueItem.MapProperties(apiRequestQueueItem);
            else
                currentRestRequestQueueItem = apiDbContext.ApiRequestQueueItems.Add(apiRequestQueueItem).Entity;

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