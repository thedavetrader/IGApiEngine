using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    #region EpicDetailCurrency
    public partial class EpicDetailCurrency
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetailCurrency()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicTick));
            Code = string.Format(Constants.InvalidEntry, nameof(Code));
            //TODO: EpicDetail = new EpicDetail();
        }

        public EpicDetailCurrency(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            MapProperties(epicDetail, currencyData);
            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = Code ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            //TODO: _ = EpicDetail ?? throw new EssentialPropertyNullReferenceException(nameof(EpicDetail));
        }
    }
    #endregion
}