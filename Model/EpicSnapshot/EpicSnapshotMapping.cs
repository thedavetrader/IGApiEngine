using System.Diagnostics.CodeAnalysis;
using dto.endpoint.prices.v3;
using IGApi.Common.Extensions;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class EpicSnapshot
    {
        public void MapProperties(
            [NotNullAttribute] string resolution,
            [NotNullAttribute] string epic,
            [NotNullAttribute] PriceSnapshot priceSnapshot
            )
        {
            Resolution = resolution;
            OpenTime = priceSnapshot.snapshotTime.FromIgString();
            CloseTime = OpenTime.AddResolution(resolution);
            OpenTimeUTC = priceSnapshot.snapshotTimeUTC.FromIgUTCString();
            CloseTimeUTC = OpenTimeUTC.AddResolution(resolution);
            Epic = epic;
            OpenBid = priceSnapshot.openPrice.bid;
            OpenAsk = priceSnapshot.openPrice.ask;
            OpenLastTraded = priceSnapshot.openPrice.lastTraded;
            CloseBid = priceSnapshot.closePrice.bid;
            CloseAsk = priceSnapshot.closePrice.ask;
            CloseLastTraded = priceSnapshot.closePrice.lastTraded;
            HighBid = priceSnapshot.highPrice.bid;
            HighAsk = priceSnapshot.highPrice.ask;
            HighLastTraded = priceSnapshot.highPrice.lastTraded;
            LowBid = priceSnapshot.lowPrice.bid;
            LowAsk = priceSnapshot.lowPrice.ask;
            LowLastTraded = priceSnapshot.lowPrice.lastTraded;
            lastTradedVolume = priceSnapshot.lastTradedVolume;
        }
    }
}
