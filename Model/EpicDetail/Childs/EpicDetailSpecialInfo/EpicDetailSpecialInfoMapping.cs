using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class EpicDetailSpecialInfo
    {
        public void MapProperties(
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] string specialInfo
            )
        {
            {
                #region parent details
                Epic = epicDetail.Epic;
                ApiLastUpdate = DateTime.UtcNow;
                EpicDetail = epicDetail;
                #endregion

                SpecialInfo = specialInfo;
            }
        }
    }
}