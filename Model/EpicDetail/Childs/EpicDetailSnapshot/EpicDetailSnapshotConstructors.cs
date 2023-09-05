using dto.endpoint.marketdetails.v2;
using IGApi.Common;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    #region EpicDetailSnapshot
    public partial class EpicDetailSnapshot
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetailSnapshot()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicDetailSnapshot));
            EpicDetail = new EpicDetail();
            MarketStatus = String.Empty;
            UpdateTime = String.Empty;
        }

        public EpicDetailSnapshot(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] MarketSnapshotData snapshotData
            )
        {
            MapProperties(epicDetail, snapshotData);
            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = snapshotData ?? throw new PrimaryKeyNullReferenceException(nameof(snapshotData));
            _ = EpicDetail ?? throw new EssentialPropertyNullReferenceException(nameof(EpicDetail));
            _ = MarketStatus ?? throw new EssentialPropertyNullReferenceException(nameof(MarketStatus));
            _ = UpdateTime ?? throw new EssentialPropertyNullReferenceException(nameof(UpdateTime));
        }
    }
    #endregion
}