using System.Diagnostics.CodeAnalysis;

using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class EpicDetailOpeningHour
    {
        public void MapProperties(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] TimeRange timeRange
            )
        {
            {
                #region parent details
                Epic = epicDetail.Epic;
                ApiLastUpdate = DateTime.UtcNow;
                EpicDetail = epicDetail;
                #endregion

                OpenTime = Utility.ConvertLocalTimeStringToUtcTimespan(timeRange.openTime);
                CloseTime = Utility.ConvertLocalTimeStringToUtcTimespan(timeRange.closeTime);
            }
        }
    }
}