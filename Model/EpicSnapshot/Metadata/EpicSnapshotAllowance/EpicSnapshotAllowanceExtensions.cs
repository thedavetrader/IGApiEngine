using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;
using dto.endpoint.prices.v3;
using IGApi.Common.Extensions;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicSnapshotAllowance? SaveEpicSnapshotAllowance(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] Allowance allowance
            )
        {
            apiDbContext.EpicSnapshotAllowances.RemoveRange(apiDbContext.EpicSnapshotAllowances);

            var newAllowanceEntry = new EpicSnapshotAllowance(allowance);
                apiDbContext.EpicSnapshotAllowances.Add(newAllowanceEntry);

            return newAllowanceEntry;
        }
    }
}