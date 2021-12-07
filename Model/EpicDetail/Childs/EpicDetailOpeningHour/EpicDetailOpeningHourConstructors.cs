using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    #region EpicDetailOpeningHour
    public partial class EpicDetailOpeningHour
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetailOpeningHour()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicTick));
            OpenTime = string.Format(Constants.InvalidEntry, nameof(OpenTime));
            CloseTime = string.Format(Constants.InvalidEntry, nameof(CloseTime));
        }

        public EpicDetailOpeningHour(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] TimeRange timeRange
            )
        {
            MapProperties(epicDetail, timeRange);
            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = EpicDetail ?? throw new EssentialPropertyNullReferenceException(nameof(EpicDetail));
            _ = OpenTime ?? throw new EssentialPropertyNullReferenceException(nameof(OpenTime));
            _ = CloseTime ?? throw new EssentialPropertyNullReferenceException(nameof(CloseTime));
        }
    }
    #endregion
}