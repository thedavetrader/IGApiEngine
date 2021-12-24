namespace IGApi.Common
{
    public static class Utility
    {
        public static void WaitFor(int millisecond)
        {
            Thread.Sleep(millisecond);
        }

        /// <summary>
        /// When the source is in string format and represents the local (as is known by IG) client time.
        /// Do not use to construct DateTime, this function does not offset for date.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan ConvertLocalTimeStringToUtcTimespan(string time)
        {
            var timeSpanTime = TimeSpan.Parse(time);

            DateTime dateTime = DateTime.Now.Date + timeSpanTime;

            var timestampLocal = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            return TimeZoneInfo.ConvertTimeToUtc(timestampLocal, TimeZoneInfo.Local).TimeOfDay;
        }

        /// <summary>
        /// Counts the number of arguments that are true.
        /// </summary>
        /// <param name="booleans"></param>
        /// <returns></returns>
        public static int CountTrue(params bool[] booleans)
        {
            return booleans.Count(t => t);
        }

        public static DateTime round_minute(DateTime dateTime, DateTimeKind kind = DateTimeKind.Utc)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, kind: kind);
        }
    }
}
