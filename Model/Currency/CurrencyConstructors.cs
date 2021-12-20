using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class Currency
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public Currency()
        {
            Code = string.Format(Constants.InvalidEntry, nameof(Currency));
        }

        public Currency(
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            MapProperties(currencyData);
            _ = Code ?? throw new PrimaryKeyNullReferenceException(nameof(Code));
        }
    }
}