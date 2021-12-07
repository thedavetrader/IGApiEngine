using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailSpecialInfo? SaveEpicDetailSpecialInfo(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] string specialInfo
            )
        {
            _ = iGApiDbContext.EpicDetailsSpecialInfo ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsSpecialInfo));

            var epicDetailSpecialInfo = Task.Run(async () => await iGApiDbContext.EpicDetailsSpecialInfo.FindAsync(epicDetail.Epic, specialInfo)).Result;

            if (epicDetailSpecialInfo is not null)
                epicDetailSpecialInfo.MapProperties(epicDetail, specialInfo);
            else
                epicDetailSpecialInfo = iGApiDbContext.EpicDetailsSpecialInfo.Add(new EpicDetailSpecialInfo(epicDetail, specialInfo)).Entity;

            return epicDetailSpecialInfo;
        }
    }
}