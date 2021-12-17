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
            Offer = l1LsPriceData.Offer;
            Bid = l1LsPriceData.Bid;
            High = l1LsPriceData.High;
            Low = l1LsPriceData.Low;
            MidOpen = l1LsPriceData.MidOpen;
            Change = l1LsPriceData.Change;
            ChangePct = l1LsPriceData.ChangePct;
            MarketDelay = l1LsPriceData.MarketDelay;
            MarketState = l1LsPriceData.MarketState ?? MarketState ?? String.Empty;
            UpdateTime = DateTime.UtcNow;
        }

        public void MapProperties(
            [NotNullAttribute] dto.endpoint.positions.get.otc.v2.MarketData marketData
            )
        {
            Epic = marketData.epic;
            Offer = marketData.offer;
            Bid = marketData.bid;
            High = marketData.high;
            Low = marketData.low;
            Change = marketData.netChange;
            ChangePct = marketData.percentageChange;
            MarketDelay = marketData.delayTime;
            MarketState = marketData.marketStatus ?? MarketState ?? String.Empty;
            UpdateTime = DateTime.UtcNow;
        }

        public void MapProperties(
            [NotNullAttribute] dto.endpoint.workingorders.get.v2.MarketData marketData
            )
        {
            Epic = marketData.epic;
            Offer = marketData.offer;
            Bid = marketData.bid;
            High = marketData.high;
            Low = marketData.low;
            Change = marketData.netChange;
            ChangePct = marketData.percentageChange;
            MarketDelay = marketData.delayTime;
            MarketState = marketData.marketStatus ?? MarketState ?? String.Empty;
            UpdateTime = DateTime.UtcNow;
        }
    }
}
