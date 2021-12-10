using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using dto.endpoint.positions.get.otc.v2;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class OpenPosition
    {
        public void MapProperties(
            [NotNullAttribute] OpenPositionData openPositionData,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string epic
            )
        {
            {
                AccountId = accountId;
                Epic = epic;
                ContractSize = openPositionData.contractSize;
                ControlledRisk = openPositionData.controlledRisk;
                CreatedDateUtc = DateTime.ParseExact(
                    openPositionData.createdDateUTC,
                    "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal);
                Currency = openPositionData.currency;
                DealId = openPositionData.dealId;
                Direction = openPositionData.direction;
                Level = openPositionData.level;
                LimitLevel = openPositionData.limitLevel;
                Size = openPositionData.size;
                StopLevel = openPositionData.stopLevel;
                TrailingStep = openPositionData.trailingStep;
                TrailingStopDistance = openPositionData.trailingStopDistance;
                ApiLastUpdate = DateTime.UtcNow;
            }
        }        
        
        public void MapProperties(
            [NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
            [NotNullAttribute] string accountId
            )
        {
            {
                AccountId = accountId;
                Epic = lsTradeSubscriptionData.epic;
                DealId = lsTradeSubscriptionData.dealId;
                Direction = lsTradeSubscriptionData.direction.ToString() ?? "[ERROR] Direction unknown.";

                decimal.TryParse(lsTradeSubscriptionData.level, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal level);
                Level = level;

                decimal.TryParse(lsTradeSubscriptionData.limitLevel, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal limitLevel);
                LimitLevel = limitLevel;

                decimal.TryParse(lsTradeSubscriptionData.size, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal size);
                Size = size;

                decimal.TryParse(lsTradeSubscriptionData.stopLevel, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal stopLevel);
                StopLevel = stopLevel;
                ApiLastUpdate = DateTime.UtcNow;
            }
        }
    }
}
