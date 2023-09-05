namespace IGApi.Common.Extensions
{
    using System.Data.SqlTypes;
    using System.Runtime.CompilerServices;
    using static Log;
    internal static partial class Extensions
    {
        public static string ToIgString(this DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static DateTime FromIgUTCString(this string value)
        {
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss", null), DateTimeKind.Utc));
        }

        public static DateTime FromIgString(this string value)
        {
            return DateTime.SpecifyKind(DateTime.ParseExact(value, "yyyy/MM/dd HH:mm:ss", null), DateTimeKind.Local);
        }
        public static DateTime AddResolution(this DateTime value, string resolution)
        {
            switch (resolution)
            {
                case "DAY": return value.AddDays(1);
                case "HOUR_2": return value.AddHours(2);
                case "HOUR_3": return value.AddHours(3);
                case "HOUR_4": return value.AddHours(4);
                case "MINUTE": return value.AddMinutes(1);
                case "MINUTE_10": return value.AddMinutes(10);
                case "MINUTE_15": return value.AddMinutes(15);
                case "MINUTE_2": return value.AddMinutes(2);
                case "MINUTE_3": return value.AddMinutes(3);
                case "MINUTE_30": return value.AddMinutes(30);
                case "MINUTE_5": return value.AddMinutes(5);
                case "MONTH": return value.AddMonths(1);
                case "SECOND": return value.AddSeconds(1);
                case "WEEK": return value.AddDays(7);
                default:
                    throw new Exception("AddResolution: Invalid resolution given.");
            }
        }
    }
}
