using dto.endpoint.marketdetails.v2;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class EpicDetailSnapshot
    {
        public void MapProperties(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] MarketSnapshotData snapshotData
            )
        {
            #region parent details
            Epic = epicDetail.Epic;
            EpicDetail = epicDetail;
            #endregion

            MarketStatus = snapshotData.marketStatus;
            UpdateTime = snapshotData.updateTime;
            Offer = snapshotData.offer;
            Bid = snapshotData.bid;
            Low = snapshotData.low;
            High = snapshotData.high;
            PercentageChange = snapshotData.percentageChange;
            NetChange = snapshotData.netChange;
            BinaryOdds = snapshotData.binaryOdds;
            DecimalPlacesFactor = snapshotData.decimalPlacesFactor;
            ScalingFactor = snapshotData.scalingFactor;
            ControlledRiskExtraSpread = snapshotData.controlledRiskExtraSpread;
            DelayTime = snapshotData.delayTime;
        }
    }
}