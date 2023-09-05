using System.Globalization;
using System.Runtime.CompilerServices;

namespace IGApi.Common
{
    internal class LogFormat
    {
        private readonly string _logFormat;

        private LogFormat(string logFormat)
        {
            _logFormat = logFormat;
        }

        public override string ToString()
        {
            return _logFormat;
        }
        
        public static LogFormat FormatOneColumns = new("{0, -50}{1, 120}");
        public static LogFormat FormatTwoColumns = new("{0, -50}{1, 60}{2, 60}");
        public static LogFormat FormatThreeColumns = new("{0, -50}{1, 40}{2, 40}{3, 40}");
        public static LogFormat FormatFourColumns = new("{0, -50}{1, 30}{2, 30}{3, 30}{4, 30}");
    }


    internal static class Log
    {
        private static string? isLockedByCaller;

        internal static string[] Columns(params string[] messages) { return messages; }

        /// <summary>
        /// Allows to get exclusive rights to write to console output stream.
        /// </summary>
        /// <param name="caller"></param>
        internal static void Lock([CallerMemberName] string? caller = null)
        {
            while (isLockedByCaller is not null)
            {
                Utility.WaitFor(15);
            }

            isLockedByCaller = caller;
        }

        /// <summary>
        /// Make console output stream available again.
        /// </summary>
        internal static void UnLock([CallerMemberName] string? caller = null)
        {
            if (isLockedByCaller == caller)
                isLockedByCaller = null;
        }

        internal static void WriteLog([CallerMemberName] string? caller = null)
        {
            Log.WriteLog(Columns(""), caller);
        }

        internal static void WriteOk(string[] messages, [CallerMemberName] string? caller = null)
        {
            Lock(caller);

            var curForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;

            Log.WriteLog(Columns(messages), caller);

            Console.ForegroundColor = curForegroundColor;

            UnLock(caller);
        }

        internal static void WriteWarning(string[] messages, [CallerMemberName] string? caller = null)
        {
            Lock(caller);

            var curForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Log.WriteLog(Columns(messages), caller);

            Console.ForegroundColor = curForegroundColor;

            UnLock(caller);
        }

        internal static void WriteError(string[] messages, [CallerMemberName] string? caller = null)
        {
            Lock(caller);

            var curForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;

            Log.WriteLog(Columns(messages), caller);

            Console.ForegroundColor = curForegroundColor;

            UnLock(caller);
        }

        internal static void WriteInformational(string[] messages, [CallerMemberName] string? caller = null)
        {
            Lock(caller);

            var curForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            Log.WriteLog(Columns(messages), caller);

            Console.ForegroundColor = curForegroundColor;

            UnLock(caller);
        }

        internal static void WriteLog(string[] messages, [CallerMemberName] string? caller = null)
        {
            switch (messages.Length)
            {
                case 1: WriteLog(LogFormat.FormatOneColumns, messages, caller); break;
                case 2: WriteLog(LogFormat.FormatTwoColumns, messages, caller); break;
                case 3: WriteLog(LogFormat.FormatThreeColumns, messages, caller); break;
                case 4: WriteLog(LogFormat.FormatFourColumns, messages, caller); break;
                case > 4: throw new Exception("Log only allows up to 4 messages.");
            }
        }

        private static void WriteLog(LogFormat logFormat, string[] messages, [CallerMemberName] string? caller = null)
        {
            while (isLockedByCaller != caller && isLockedByCaller is not null) { Utility.WaitFor(100); }

            string[] finalMessages = new string[messages.Length + 1];
            finalMessages[0] = $"[{DateTime.UtcNow}][{caller}]";
            Array.Copy(messages, 0, finalMessages, 1, messages.Length);

            Console.WriteLine(string.Format(
                provider: CultureInfo.InvariantCulture,
                format: logFormat.ToString(),
                args: finalMessages));
        }

        internal static void WriteException(Exception ex, [CallerMemberName] string? caller = null)
        {
            WriteError(Columns(ex.ToString()), caller);
        }
    }
}