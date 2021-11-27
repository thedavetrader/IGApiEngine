using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApiEngine.Common
{
    internal static class Log
    {
        internal static void WriteLine(string? message)
        {
            Console.WriteLine($"[{DateTime.UtcNow}]" + message);
        }
    }
}