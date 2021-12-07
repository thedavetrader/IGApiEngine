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
    }
}
