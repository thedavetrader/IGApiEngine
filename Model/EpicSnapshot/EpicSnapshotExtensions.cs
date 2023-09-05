using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;
using dto.endpoint.prices.v3;
using IGApi.Common.Extensions;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicSnapshot? SaveEpicSnapshot(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] string resolution,
            [NotNullAttribute] string epic,
            [NotNullAttribute] PriceSnapshot priceSnapshot
            )
        {
            var currentEpicSnapshot = Task.Run(async () => await apiDbContext.EpicSnapshots.FindAsync(
                resolution,
                priceSnapshot.snapshotTimeUTC.FromIgUTCString(),
                epic)).Result;

            if (currentEpicSnapshot is not null)
                currentEpicSnapshot.MapProperties(resolution, epic, priceSnapshot);
            else
                currentEpicSnapshot = apiDbContext.EpicSnapshots.Add(new EpicSnapshot(resolution, epic, priceSnapshot)).Entity;

            return currentEpicSnapshot;
        }
    }
}