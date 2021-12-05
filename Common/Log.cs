using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common
{
    internal static class Log
    {
        public static string FormatString = "{0, -50} {1, -50} {2, 50} {3, 50}";

        internal static void WriteLine(string? message)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]" + message);
        }

        internal static void WriteException(Exception ex, string caller)
        {
            Console.WriteLine($"[{DateTime.UtcNow}][{caller}] " + ex.ToString());
        }
    }
}