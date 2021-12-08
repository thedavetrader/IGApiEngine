using System.Diagnostics.CodeAnalysis;

using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    public partial class Currency
    {
        public void MapProperties(
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            {
                ApiLastUpdate = DateTime.UtcNow;

                Code = currencyData.code;
                Symbol = currencyData.symbol;
                BaseExchangeRate = currencyData.baseExchangeRate;
                IsDefault = currencyData.isDefault;
            }
        }
    }
}