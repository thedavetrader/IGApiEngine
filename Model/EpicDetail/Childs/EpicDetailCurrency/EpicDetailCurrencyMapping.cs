using System.Diagnostics.CodeAnalysis;

using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    public partial class EpicDetailCurrency
    {
        public void MapProperties(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            {
                #region parent details
                Epic = epicDetail.Epic;
                ApiLastUpdate = DateTime.UtcNow;
                EpicDetail = epicDetail;
                #endregion

                Code = currencyData.code;
                Symbol = currencyData.symbol;
                BaseExchangeRate = currencyData.baseExchangeRate;
                IsDefault = currencyData.isDefault;
            }
        }
    }
}