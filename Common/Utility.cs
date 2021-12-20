namespace IGApi.Common
{
    public static class Utility
    {
        public static void WaitFor(int millisecond)
        {
            var timer = new System.Timers.Timer(millisecond);
            timer.Elapsed += delegate { timer.Stop(); };
            timer.Start();
        }

        /// <summary>
        /// When the source is in string format and represents the local (as is known by IG) client time.
        /// Do not use to construct DateTime, this function does not offset for date.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan ConvertLocalTimeStringToUtcTimespan(string time)
        {
            return TimeSpan.Parse(time) - DateTimeOffset.Now.Offset;
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
    }
}
