using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApiEngine.Common
{
    internal static class Extensions
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await Task.Run(task.Start);
            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString());
            }
        }
    }
}
