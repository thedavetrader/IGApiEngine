using System.Diagnostics.CodeAnalysis;
using dto.endpoint.prices.v3;
using IGApi.Common.Extensions;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class EpicSnapshotAllowance
    {
        public void MapProperties(
            [NotNullAttribute] Allowance allowance
            )
        {

            AllowanceExpiry = allowance.allowanceExpiry;
            RemainingAllowance = allowance.remainingAllowance;
            TotalAllowance = allowance.totalAllowance;
        }
    }
}
