using System.Diagnostics.CodeAnalysis;

using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    public partial class EpicDetailCurrency
    {
        public void MapProperties(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] Currency currency
            )
        {
            #region parent details
            Epic = epicDetail.Epic;
            ApiLastUpdate = DateTime.UtcNow;
            EpicDetail = epicDetail;
            Currency = currency;
            #endregion

            Code = currency.Code;
        }
    }
}