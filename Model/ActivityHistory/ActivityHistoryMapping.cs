using dto.endpoint.accountactivity.activity_v3;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class ActivityHistory
    {
        public void MapProperties(
            [NotNullAttribute] Activity_v3 activity
            )
        {
            Timestamp = activity.GetTimeStamp();
            Epic = activity.epic;
            Period = activity.period;
            DealId = activity.dealId;
            Channel = activity.channel;
            Type = activity.type;
            Status = activity.status;
            Description = activity.description;

            // DETAILS
            DealReference = activity.details.dealReference;
            MarketName = activity.details.marketName;
            GoodTillDate = activity.details.goodTillDate;
            Currency = activity.details.currency;
            Size = activity.details.size;
            Direction = activity.details.direction;
            Level = activity.details.level;
            StopLevel = activity.details.stopLevel;
            StopDistance = activity.details.stopDistance;
            GuaranteedStop = activity.details.guaranteedStop;
            TrailingStopDistance = activity.details.trailingStopDistance;
            TrailingStep = activity.details.trailingStep;
            LimitLevel = activity.details.limitLevel;
            LimitDistance = activity.details.limitDistance;
        }
    }
}