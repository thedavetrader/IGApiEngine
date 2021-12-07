using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailOpeningHour? SaveEpicDetailOpeningHour(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] TimeRange timeRange
            )
        {
            _ = iGApiDbContext.EpicDetailsOpeningHour ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsOpeningHour));

            var epicDetailOpeningHour = Task.Run(async () => await iGApiDbContext.EpicDetailsOpeningHour.FindAsync(epicDetail.Epic, timeRange.openTime)).Result;

            if (epicDetailOpeningHour is not null)
                epicDetailOpeningHour.MapProperties(epicDetail, timeRange);
            else
                epicDetailOpeningHour = iGApiDbContext.EpicDetailsOpeningHour.Add(new EpicDetailOpeningHour(epicDetail, timeRange)).Entity;

            return epicDetailOpeningHour;
        }
    }
}