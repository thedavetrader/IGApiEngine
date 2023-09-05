using dto.endpoint.marketdetails.v2;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailSnapshot? SaveEpicDetailSnapshot(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] MarketSnapshotData snapshotData
            )
        {
            var EpicDetailSnapshot = Task.Run(async () => await apiDbContext.EpicDetailSnapshot.FindAsync(epicDetail.Epic)).Result;

            if (EpicDetailSnapshot is not null)
                EpicDetailSnapshot.MapProperties(epicDetail, snapshotData);
            else
                EpicDetailSnapshot = apiDbContext.EpicDetailSnapshot.Add(new EpicDetailSnapshot(epicDetail, snapshotData)).Entity;

            return EpicDetailSnapshot;
        }
    }
}