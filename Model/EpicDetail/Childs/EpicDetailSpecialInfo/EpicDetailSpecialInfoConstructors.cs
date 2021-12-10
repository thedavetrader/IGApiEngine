using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    #region EpicDetailSpecialInfo
    public partial class EpicDetailSpecialInfo
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetailSpecialInfo()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(EpicDetailSpecialInfo));
            SpecialInfo= string.Format(Constants.InvalidEntry, nameof(EpicDetailSpecialInfo));
            EpicDetail = new EpicDetail();
        }

        public EpicDetailSpecialInfo(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] string specialInfo
            )
        {
            MapProperties(epicDetail, specialInfo);
            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
            _ = SpecialInfo ?? throw new PrimaryKeyNullReferenceException(nameof(SpecialInfo));
            _ = EpicDetail ?? throw new EssentialPropertyNullReferenceException(nameof(EpicDetail));
        }
    }
    #endregion
}