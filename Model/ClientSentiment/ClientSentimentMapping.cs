using System.Diagnostics.CodeAnalysis;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class ClientSentiment
    {
        public void MapProperties(
            [NotNullAttribute] dto.endpoint.clientsentiment.ClientSentiment clientSentiment
            )
        {
            MarketId = clientSentiment.marketId;
            LongPositionPercentage = clientSentiment.longPositionPercentage;
            ShortPositionPercentage = clientSentiment.shortPositionPercentage;
            ApiLastUpdate = DateTime.UtcNow;
        }
    }
}
