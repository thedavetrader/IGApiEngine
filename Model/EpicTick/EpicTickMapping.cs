using System.Diagnostics.CodeAnalysis;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class EpicTick
    {
        public void MapProperties(
            [NotNullAttribute] L1LsPriceData l1LsPriceData,
            [NotNullAttribute] string epic
            )
        {
            Epic = epic;
            Offer = l1LsPriceData.Offer ?? Offer;
            Bid = l1LsPriceData.Bid ?? Bid;
            High = l1LsPriceData.High ?? High;
            Low = l1LsPriceData.Low ?? Low;
            MidOpen = l1LsPriceData.MidOpen ?? MidOpen;
            Change = l1LsPriceData.Change ?? Change;
            ChangePct = l1LsPriceData.ChangePct ?? ChangePct;
            MarketDelay = l1LsPriceData.MarketDelay ?? MarketDelay;
            MarketState = l1LsPriceData.MarketState ?? MarketState ?? String.Empty;
            UpdateTime = DateTime.UtcNow;
        }

        public void MapProperties(
            [NotNullAttribute] dto.endpoint.positions.get.otc.v2.MarketData marketData
            )
        {
            Epic = marketData.epic;
            Offer = marketData.offer ?? Offer;
            Bid = marketData.bid ?? Bid;
            High = marketData.high ?? High;
            Low = marketData.low ?? Low;
            Change = marketData.netChange ?? Change;
            ChangePct = marketData.percentageChange ?? ChangePct;
            MarketDelay = marketData.delayTime;
            MarketState = marketData.marketStatus ?? MarketState ?? String.Empty;
            UpdateTime = DateTime.UtcNow;
        }

        public void MapProperties(
            [NotNullAttribute] dto.endpoint.workingorders.get.v2.MarketData marketData
            )
        {
            Epic = marketData.epic;
            Offer = marketData.offer ?? Offer;
            Bid = marketData.bid ?? Bid;
            High = marketData.high ?? High;
            Low = marketData.low ?? Low;
            Change = marketData.netChange ?? Change;
            ChangePct = marketData.percentageChange ?? ChangePct;
            MarketDelay = marketData.delayTime;
            MarketState = marketData.marketStatus ?? MarketState ?? String.Empty;
            UpdateTime = DateTime.UtcNow;
        }
    }
}
