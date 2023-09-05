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
            AccountId = accountId;
            Epic = epic;
            ContractSize = openPositionData.contractSize;
            ControlledRisk = openPositionData.controlledRisk;
            CreatedDateUtc = openPositionData.createdDateUTC is null ?
                CreatedDateUtc :
                DateTime.ParseExact(
                openPositionData.createdDateUTC,
                "yyyy-MM-dd'T'HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal |
                DateTimeStyles.AdjustToUniversal);
            Currency = openPositionData.currency ?? Currency;
            DealId = openPositionData.dealId ?? DealId;
            Direction = openPositionData.direction ?? Direction;
            Level = openPositionData.level;
            LimitLevel = openPositionData.limitLevel;
            Size = openPositionData.size;
            StopLevel = openPositionData.stopLevel;
            TrailingStep = openPositionData.trailingStep;
            TrailingStopDistance = openPositionData.trailingStopDistance;
            ApiLastUpdate = DateTime.UtcNow;
            DealReference = openPositionData.dealReference;
        }

        public void MapProperties(
            [NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
            [NotNullAttribute] string accountId
            )
        {
            AccountId = accountId;
            Epic = lsTradeSubscriptionData.epic;
            DealId = lsTradeSubscriptionData.dealId;
            Direction = lsTradeSubscriptionData.direction.ToString() ?? "[ERROR] Direction unknown.";
            decimal.TryParse(lsTradeSubscriptionData.level, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal level);
            Level = level;
            decimal.TryParse(lsTradeSubscriptionData.limitLevel, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal limitLevel);
            LimitLevel = limitLevel == 0 ? null : limitLevel;
            decimal.TryParse(lsTradeSubscriptionData.size, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal size);
            Size = size;
            decimal.TryParse(lsTradeSubscriptionData.stopLevel, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal stopLevel);
            StopLevel = stopLevel == 0 ? null : stopLevel;
            ApiLastUpdate = DateTime.UtcNow;
            DealReference = lsTradeSubscriptionData.dealReference;

            //"2022-01-05T12:49:25.873"
            CreatedDateUtc =
                DateTime.ParseExact(
                lsTradeSubscriptionData.timestamp,
                "yyyy-MM-dd'T'HH:mm:ss.fff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal |
                DateTimeStyles.AdjustToUniversal);
        }
    }
}
