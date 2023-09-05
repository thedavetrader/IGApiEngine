using IGApi.Common;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.prices.v3;

namespace IGApi.Model
{
    public partial class EpicSnapshotAllowance
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicSnapshotAllowance()
        { 
        }

        /// <summary>
        /// For creating new accounts using L1LsPriceData
        /// </summary>
        /// <param name="l1LsPriceData"></param>
        /// <param name="epic"></param>
        public EpicSnapshotAllowance(
            [NotNullAttribute] Allowance allowance
            )
        {
            MapProperties(allowance);
        }
    }
}