using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IGApi.Common;
using dto.endpoint.accountactivity.activity;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ActivityHistory? SaveActivityHistory(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] Activity activity
            )
        {
            _ = iGApiDbContext.ActivitiesHistory ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ActivitiesHistory));

            var currentActivity = Task.Run(async () => await iGApiDbContext.ActivitiesHistory.FindAsync(activity.GetTimestamp(), activity.dealId)).Result;

            if (currentActivity is not null)
                currentActivity.MapProperties(activity);
            else
                currentActivity = iGApiDbContext.ActivitiesHistory.Add(new ActivityHistory(activity)).Entity;

            return currentActivity;
        }

        public static DateTime GetTimestamp(this Activity activity)
        {

            if (
                DateTime.TryParseExact(
                activity.date,
                "dd/MM/yy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal |
                DateTimeStyles.AdjustToUniversal,
                out DateTime date)

                &&

                TimeSpan.TryParseExact(
                activity.time,
                "hh\\:mm",
                CultureInfo.InvariantCulture,
                TimeSpanStyles.None,
                out TimeSpan time))
            {
                var timestampLocal = DateTime.SpecifyKind(date + time, DateTimeKind.Local);
                return TimeZoneInfo.ConvertTimeToUtc(timestampLocal, TimeZoneInfo.Local); ;
            }
            else
                throw new EssentialPropertyNullReferenceException(nameof(GetTimestamp));
        }
    }
}