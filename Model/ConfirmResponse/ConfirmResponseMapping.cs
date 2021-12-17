using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace IGApi.Model
{
    public partial class ConfirmResponse
    {
        public void MapProperties(
            [NotNullAttribute] dto.endpoint.confirms.ConfirmsResponse confirmsResponse
            )
        {
            DealId = confirmsResponse.dealId;
            DealReference = confirmsResponse.dealReference;
            DealStatus = confirmsResponse.dealStatus;
            Direction = confirmsResponse.direction;
            Epic = confirmsResponse.epic;
            Expiry = confirmsResponse.expiry;
            GuaranteedStop = confirmsResponse.guaranteedStop;
            Level = confirmsResponse.level;
            LimitDistance = confirmsResponse.limitDistance;
            LimitLevel = confirmsResponse.limitLevel;
            Reason = confirmsResponse.reason;
            Size = confirmsResponse.size;
            Status = confirmsResponse.status;
            StopDistance = confirmsResponse.stopDistance;
            StopLevel = confirmsResponse.stopLevel;
            Timestamp = DateTime.UtcNow;
            AffectedDeals = JsonConvert.SerializeObject(confirmsResponse.affectedDeals, Formatting.None);
        }
    }
}