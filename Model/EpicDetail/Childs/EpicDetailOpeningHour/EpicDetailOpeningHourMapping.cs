using System.Diagnostics.CodeAnalysis;

using dto.endpoint.marketdetails.v2;

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

                //TODO: Convert from local time to utc time
                //TimeSpan timeSpan = new TimeSpan(2, 14, 18);
                //var MinDate = DateTime.Today.to;


                OpenTime = timeRange.openTime;
                CloseTime = timeRange.closeTime;
            }
        }
    }
}