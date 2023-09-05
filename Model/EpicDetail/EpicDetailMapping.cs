using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;
using IGApi.Common.Extensions;

namespace IGApi.Model
{
    public partial class EpicDetail
    {
        public void MapProperties(
            [NotNullAttribute] InstrumentData instrumentData,
            DealingRulesData? dealingRulesData,
            MarketSnapshotData? marketSnapshotData
            )
        {
            ChartCode = instrumentData.chartCode;
            decimal.TryParse(instrumentData.contractSize, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal contractSize);
            ContractSize = contractSize.TryParseSqlDecimal(instrumentData.epic);
            ControlledRiskAllowed = instrumentData.controlledRiskAllowed;
            Country = instrumentData.country;
            Epic = instrumentData.epic;
            Expiry = instrumentData.expiry;
            ForceOpenAllowed = instrumentData.forceOpenAllowed;
            LotSize = instrumentData.lotSize.TryParseSqlDecimal(instrumentData.epic);
            MarginFactor = instrumentData.marginFactor.TryParseSqlDecimal(instrumentData.epic);
            MarginFactorUnit = instrumentData.marginFactorUnit;
            MarketId = instrumentData.marketId;
            Name = instrumentData.name;
            NewsCode = instrumentData.newsCode;
            OnePipMeans = instrumentData.onePipMeans;
            SprintMarketsMaximumExpiryTime = instrumentData.sprintMarketsMaximumExpiryTime;
            SprintMarketsMinimumExpiryTime = instrumentData.sprintMarketsMinimumExpiryTime;
            StopsLimitsAllowed = instrumentData.stopsLimitsAllowed;
            StreamingPricesAvailable = instrumentData.streamingPricesAvailable;
            Type = instrumentData.type;
            PositionSizeUnit = instrumentData.unit;
            ValueOfOnePip = instrumentData.valueOfOnePip;
            ApiLastUpdate = DateTime.UtcNow;

            #region Class contained properties
            if (instrumentData.expiryDetails is not null)
            {
                ExpirylastDealingDate = DateTime.ParseExact(
                    instrumentData.expiryDetails.lastDealingDate,
                    "yyyy-MM-dd'T'HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal);
                ExpirysettlementInfo = instrumentData.expiryDetails.settlementInfo;
            }

            if (instrumentData.rolloverDetails is not null)
            {
                LastRolloverTime = DateTime.ParseExact(
                    instrumentData.rolloverDetails.lastRolloverTime,
                    "yyyy-MM-dd'T'HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal);
                RolloverInfo = instrumentData.rolloverDetails.rolloverInfo;
            }

            if (instrumentData.slippageFactor is not null)
            {
                SlippageFactorUnit = instrumentData.slippageFactor.unit;
                SlippageFactorValue = instrumentData.slippageFactor.value.TryParseSqlDecimal(instrumentData.epic);
            }

            if (dealingRulesData is not null)
            {
                DealingRuleMarketOrderPreference = dealingRulesData.marketOrderPreference;
                DealingRuleTrailingStopsPreference = dealingRulesData.trailingStopsPreference;

                // Unit
                DealingRuleUnitMaxStopOrLimitDistance = dealingRulesData.maxStopOrLimitDistance.unit;
                DealingRuleUnitMinControlledRiskStopDistance = dealingRulesData.minControlledRiskStopDistance.unit;
                DealingRuleUnitMinDealSize = dealingRulesData.minDealSize.unit;
                DealingRuleUnitMinNormalStopOrLimitDistance = dealingRulesData.minNormalStopOrLimitDistance.unit;
                DealingRuleUnitMinStepDistance = dealingRulesData.minStepDistance.unit;

                // Value
                DealingRuleValueMaxStopOrLimitDistance = dealingRulesData.maxStopOrLimitDistance.value.TryParseSqlDecimal(instrumentData.epic);
                DealingRuleValueMinControlledRiskStopDistance = dealingRulesData.minControlledRiskStopDistance.value.TryParseSqlDecimal(instrumentData.epic);
                DealingRuleValueMinDealSize = dealingRulesData.minDealSize.value.TryParseSqlDecimal(instrumentData.epic);
                DealingRuleValueMinNormalStopOrLimitDistance = dealingRulesData.minNormalStopOrLimitDistance.value.TryParseSqlDecimal(instrumentData.epic);
                DealingRuleValueMinStepDistance = dealingRulesData.minStepDistance.value.TryParseSqlDecimal(instrumentData.epic);
            }

            if (marketSnapshotData is not null)
            {
                MarketStatus = marketSnapshotData.marketStatus;
                NetChange = marketSnapshotData.netChange;
                PercentageChange = marketSnapshotData.percentageChange;
                UpdateTime = marketSnapshotData.updateTime;
                DelayTime = marketSnapshotData.delayTime;
                Bid = marketSnapshotData.bid;
                Offer = marketSnapshotData.offer;
                High = marketSnapshotData.high;
                Low = marketSnapshotData.low;
                BinaryOdds = marketSnapshotData.binaryOdds;
                DecimalPlacesFactor = marketSnapshotData.decimalPlacesFactor;
                ScalingFactor = marketSnapshotData.scalingFactor;
                ControlledRiskExtraSpread = marketSnapshotData.controlledRiskExtraSpread;
            }
            #endregion
        }
    }
}
