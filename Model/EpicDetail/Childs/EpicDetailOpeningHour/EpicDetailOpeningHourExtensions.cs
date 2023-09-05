using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailOpeningHour? SaveEpicDetailOpeningHour(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] TimeRange timeRange
            )
        {
            var openTime = Utility.ConvertLocalTimeStringToUtcTimespan(timeRange.openTime);
            var epicDetailOpeningHour = Task.Run(async () => await apiDbContext.EpicDetailsOpeningHour.FindAsync(epicDetail.Epic, openTime)).Result;

            if (epicDetailOpeningHour is not null)
                epicDetailOpeningHour.MapProperties(epicDetail, timeRange);
            else
                epicDetailOpeningHour = apiDbContext.EpicDetailsOpeningHour.Add(new EpicDetailOpeningHour(epicDetail, timeRange)).Entity;

            return epicDetailOpeningHour;
        }
    }
}