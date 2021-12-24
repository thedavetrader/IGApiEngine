using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailSpecialInfo? SaveEpicDetailSpecialInfo(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] string specialInfo
            )
        {
            _ = apiDbContext.EpicDetailsSpecialInfo ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetailsSpecialInfo));

            var epicDetailSpecialInfo = Task.Run(async () => await apiDbContext.EpicDetailsSpecialInfo.FindAsync(epicDetail.Epic, specialInfo)).Result;

            if (epicDetailSpecialInfo is not null)
                epicDetailSpecialInfo.MapProperties(epicDetail, specialInfo);
            else
                epicDetailSpecialInfo = apiDbContext.EpicDetailsSpecialInfo.Add(new EpicDetailSpecialInfo(epicDetail, specialInfo)).Entity;

            return epicDetailSpecialInfo;
        }
    }
}