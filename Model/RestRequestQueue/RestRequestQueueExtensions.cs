﻿using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static IGApiDbContext SaveRestRequestQueue(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] RestRequestQueue restRequestQueueItem
            )
        {
            _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

            var existingrestRequestQueueItem = Task.Run(async () => await iGApiDbContext.RestRequestQueue.FindAsync(restRequestQueueItem.Id)).Result;

            if (existingrestRequestQueueItem is not null)
                iGApiDbContext.Entry(existingrestRequestQueueItem).CurrentValues.SetValues(restRequestQueueItem);
            else
                iGApiDbContext.RestRequestQueue.Add(restRequestQueueItem);

            return iGApiDbContext;
        }
    }
}