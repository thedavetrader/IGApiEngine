using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    public partial class EpicDetail
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetail()
        { Epic = string.Format(Constants.InvalidEntry, nameof(EpicDetail)); }

        public EpicDetail(
            [NotNullAttribute] InstrumentData instrumentData,
            DealingRulesData? dealingRulesData,
            MarketSnapshotData? marketSnapshotData
            )
        {
            MapProperties(instrumentData, dealingRulesData, marketSnapshotData);

            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
        }
    }
}