using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using dto.endpoint.positions.get.otc.v2;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class EpicDetail
    {
        public void MapProperties(
            [NotNullAttribute] dto.endpoint.marketdetails.v2.InstrumentData instrumentData
            )
        {
            {
                ChartCode = instrumentData.chartCode;
                decimal.TryParse(instrumentData.contractSize, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal contractSize);
                ContractSize = contractSize;
                ControlledRiskAllowed = instrumentData.controlledRiskAllowed;
                Country = instrumentData.country; 
                //TODO: Currencies = instrumentData.currencies;
                Epic = instrumentData.epic;
                Expiry = instrumentData.expiry;
                //TODO: ExpiryDetails = instrumentData.expiryDetails;
                ForceOpenAllowed = instrumentData.forceOpenAllowed;
                LotSize = instrumentData.lotSize;
                //TODO: MarginDepositBands = instrumentData.marginDepositBands;
                MarginFactor = instrumentData.marginFactor;
                MarginFactorUnit = instrumentData.marginFactorUnit;
                MarketId = instrumentData.marketId;
                Name = instrumentData.name;
                NewsCode = instrumentData.newsCode;
                OnePipMeans = instrumentData.onePipMeans;
                //TODO: OpeningHours = instrumentData.openingHours;
                //TODO: RolloverDetails = instrumentData.rolloverDetails;
                //TODO: SlippageFactor = instrumentData.slippageFactor;
                //TODO: SpecialInfo = instrumentData.specialInfo;
                SprintMarketsMaximumExpiryTime = instrumentData.sprintMarketsMaximumExpiryTime;
                SprintMarketsMinimumExpiryTime = instrumentData.sprintMarketsMinimumExpiryTime;
                StopsLimitsAllowed = instrumentData.stopsLimitsAllowed;
                StreamingPricesAvailable = instrumentData.streamingPricesAvailable;
                Type = instrumentData.type;
                PositionSizeUnit = instrumentData.unit;
                ValueOfOnePip = instrumentData.valueOfOnePip;
            }
        }
    }
}
