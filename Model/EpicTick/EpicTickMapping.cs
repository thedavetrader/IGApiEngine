using System.Diagnostics.CodeAnalysis;
using dto.endpoint.positions.get.otc.v2;
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
                MarketState = l1LsPriceData.MarketState;
                UpdateTime = DateTime.Now;
            }
        }

        public void MapProperties(
            [NotNullAttribute] MarketData marketData
            )
        {
            {
                Epic = marketData.epic;
                Offer = marketData.offer;
                Bid = marketData.bid;
                High = marketData.high;
                Low = marketData.low;
                Change = marketData.netChange;
                ChangePct = marketData.percentageChange;
                MarketDelay = marketData.delayTime;
                MarketState = marketData.marketStatus;
                UpdateTime = DateTime.Now;
            }
        }

    }
}
