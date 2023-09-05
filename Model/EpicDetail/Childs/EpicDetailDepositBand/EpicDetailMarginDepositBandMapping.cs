using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using dto.endpoint.marketdetails.v2;
using IGApi.Common.Extensions;

namespace IGApi.Model
{
    public partial class EpicDetailMarginDepositBand
    {
        public void MapProperties(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] DepositBand DepositBand
            )
        {
            {
                #region parent details
                Epic = epicDetail.Epic;
                ApiLastUpdate = DateTime.UtcNow;
                EpicDetail = epicDetail;
                #endregion

                Currency = DepositBand.currency;
                Margin = DepositBand.margin;
                Min = DepositBand.min.TryParseSqlDecimal(epicDetail.Epic) ?? throw new EssentialPropertyNullReferenceException(nameof(DepositBand.min)); ;
                Max = DepositBand.max.TryParseSqlDecimal(epicDetail.Epic);
            }
        }
    }
}