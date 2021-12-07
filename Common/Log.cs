using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Common
{
    internal static class Log
    {
        public static string FormatFourColumns = "{0, -50} {1, -50} {2, 50} {3, 50}";
        public static string FormatTwoColumns = "{0, -100} {1, 100}";

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