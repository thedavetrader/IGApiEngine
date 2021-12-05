using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class EpicDetail
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public EpicDetail()
        { Epic = string.Format(Constants.InvalidEntry, nameof(EpicDetail)); }

        public EpicDetail(
            [NotNullAttribute] dto.endpoint.marketdetails.v2.InstrumentData instrumentData
            )
        {
            MapProperties(instrumentData);

            _ = Epic ?? throw new PrimaryKeyNullReferenceException(nameof(Epic));
        }

        #region EpicDetailSpecialInfo
        public partial class EpicDetailSpecialInfo
        {
            /// <summary>
            /// Also provide normal constructor for EF-Core.
            /// </summary>
            [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
            public EpicDetailSpecialInfo()
            {
                Epic = string.Format(Constants.InvalidEntry, nameof(EpicTick));
                EpicDetail = new EpicDetail();
            }

            //TODO: EpicDetailSpecialInfo
            public EpicDetailSpecialInfo(EpicDetail epicDetail, string todo)
            {
                Epic = epicDetail.Epic;
                SpecialInfo = todo;
                EpicDetail = epicDetail;
            }
        }
        #endregion
    }
}