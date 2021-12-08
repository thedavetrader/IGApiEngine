using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    public partial class EpicDetail
    {
        public void MapProperties(
            [NotNullAttribute] InstrumentData instrumentData
            )
        {
            {
                ChartCode = instrumentData.chartCode;
                decimal.TryParse(instrumentData.contractSize, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal contractSize);
                ContractSize = contractSize;
                ControlledRiskAllowed = instrumentData.controlledRiskAllowed;
                Country = instrumentData.country; 
                Epic = instrumentData.epic;
                Expiry = instrumentData.expiry;
                ForceOpenAllowed = instrumentData.forceOpenAllowed;
                LotSize = instrumentData.lotSize;
                MarginFactor = instrumentData.marginFactor;
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

                if(instrumentData.slippageFactor is not null)
                {
                    SlippageFactorUnit = instrumentData.slippageFactor.unit;
                    SlippageFactorValue = instrumentData.slippageFactor.value;
                }
                #endregion
            }
        }
    }
}
