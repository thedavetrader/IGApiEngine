using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    #region EpicDetailDepositBand
    public partial class EpicDetailMarginDepositBand
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetailMarginDepositBand()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicDetailMarginDepositBand));
            Currency = string.Format(Constants.InvalidEntry, nameof(EpicDetailMarginDepositBand));
            EpicDetail = new EpicDetail();
        }

        public EpicDetailMarginDepositBand(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] DepositBand DepositBand
            )
        {
            MapProperties(epicDetail, DepositBand);
            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = Currency ?? throw new PrimaryKeyNullReferenceException(nameof(Currency));
            _ = EpicDetail ?? throw new EssentialPropertyNullReferenceException(nameof(EpicDetail));
        }
    }
    #endregion
}