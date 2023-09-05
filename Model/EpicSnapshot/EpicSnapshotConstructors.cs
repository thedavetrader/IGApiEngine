using IGApi.Common;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.prices.v3;

namespace IGApi.Model
{
    public partial class EpicSnapshot
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicSnapshot()
        { 
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicSnapshot)); 
            Resolution = string.Format(Constants.InvalidEntry, nameof(Resolution)); 
        }

        /// <summary>
        /// For creating new accounts using L1LsPriceData
        /// </summary>
        /// <param name="l1LsPriceData"></param>
        /// <param name="epic"></param>
        public EpicSnapshot(
            [NotNullAttribute] string resolution,
            [NotNullAttribute] string epic,
            [NotNullAttribute] PriceSnapshot priceSnapshot
            )
        {
            MapProperties(resolution, epic, priceSnapshot);

            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = Resolution ?? throw new PrimaryKeyNullReferenceException(nameof(Resolution));
        }
    }
}