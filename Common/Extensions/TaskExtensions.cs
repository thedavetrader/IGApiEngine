using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common.Extensions
{
    internal static class Extensions
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await Task.Run(task.Start);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(FireAndForget));
                throw;
            }
        }
    }
}
