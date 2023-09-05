using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IGApi.Common;
using dto.endpoint.accountactivity.activity_v3;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ActivityHistory? SaveActivityHistory(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] Activity_v3 activity
            )
        {
            var currentActivity = Task.Run(async () => await apiDbContext.ActivitiesHistory.FindAsync(activity.GetTimeStamp(), activity.dealId)).Result;

            if (currentActivity is not null)
                currentActivity.MapProperties(activity);
            else
                currentActivity = apiDbContext.ActivitiesHistory.Add(new ActivityHistory(activity)).Entity;

            return currentActivity;
        }

        public static DateTime GetTimeStamp(this Activity_v3 activity)
        {
            return DateTime.Parse(
                activity.date,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal); //TODO: Local or UTC?
        }
    }
}