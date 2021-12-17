using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IGApi.Common;
using dto.endpoint.accountactivity.activity;

namespace IGApi.Model
{
    public partial class
ActivityHistory
    {
        public void MapProperties(
            [NotNullAttribute] Activity activity
            )
        {
            ActionStatus = activity.actionStatus;
            Activity = activity.activity;
            ActivityHistoryId = activity.activityHistoryId;
            Channel = activity.channel;
            Currency = activity.currency;
            DealId = activity.dealId;
            Epic = activity.epic;
            decimal.TryParse(activity.level, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal level);
            Level = level;
            decimal.TryParse(activity.stop, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal stop);
            Stop = stop;
            decimal.TryParse(activity.limit, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal limit);
            Limit = limit;
            MarketName = activity.marketName;
            Period = activity.period;
            Result = activity.result;
            Size = activity.size;
            StopType = activity.stopType;
            Timestamp = activity.GetTimestamp();
            Reference = DealId.Substring(7, 8);
        }
    }
}