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
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicDetailCurrency));
            Code = string.Format(Constants.InvalidEntry, nameof(EpicDetailCurrency));
            EpicDetail = new EpicDetail();
            Currency = new Currency();
        }

        public EpicDetailCurrency(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] Currency currency
            )
        {
            MapProperties(epicDetail, currency);
            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = Code ?? throw new PrimaryKeyNullReferenceException(nameof(Code));
            _ = EpicDetail ?? throw new EssentialPropertyNullReferenceException(nameof(EpicDetail));
            _ = Currency ?? throw new EssentialPropertyNullReferenceException(nameof(Currency));
        }
    }
    #endregion
}