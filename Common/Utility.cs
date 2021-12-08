using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common
{
    public static class Utility
    {
        public static void DelayAction(int millisecond)
        {
            var timer = new System.Timers.Timer(millisecond);
            timer.Elapsed += delegate { timer.Stop(); };
            timer.Start();
        }

        /// <summary>
        /// When the source is in string format and represents the local (as is known by IG) client time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan ConvertLocalTimeStringToUtcTimespan(string time)
        {
            return TimeSpan.Parse(time) - DateTimeOffset.Now.Offset;
        }
    }
}
