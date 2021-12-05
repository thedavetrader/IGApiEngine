using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class EpicTick
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicTick()
        { Epic = string.Format(Constants.InvalidEntry, nameof(EpicTick)); }

        /// <summary>
        /// For creating new accounts using L1LsPriceData
        /// </summary>
        /// <param name="l1LsPriceData"></param>
        /// <param name="epic"></param>
        public EpicTick(
            [NotNullAttribute] L1LsPriceData l1LsPriceData,
            [NotNullAttribute] string epic
            )
        {
            MapProperties(l1LsPriceData, epic);

            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
        }

        /// <summary>
        /// For creating new accounts using dto.endpoint.positions.get.otc.v2.MarketData
        /// </summary>
        /// <param name="marketData"></param>
        /// <param name="epic"></param>
        public EpicTick(
            [NotNullAttribute] MarketData marketData
            )
        {
            MapProperties(marketData);

            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
        }
    }
}