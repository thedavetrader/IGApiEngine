using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    using static Log;

    internal static partial class DtoModelExtensions
    {
        [Obsolete("Use natively compiled stored prcedure call \"ApiRequestQueueItem.SaveApiRequestQueueItem\" for saving to RequestQueue.", true)]
        public static ApiRequestQueueItem? SaveApiRequestQueueItem(
            [NotNullAttribute] string connectionString,
            [NotNullAttribute] ApiRequestQueueItem apiRequestQueueItem
            )
        {
            throw new NotImplementedException("Use natively compiled stored prcedure call \"ApiRequestQueueItem.SaveApiRequestQueueItem\" for saving to RequestQueue.");
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