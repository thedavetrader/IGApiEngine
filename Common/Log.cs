using System.Globalization;
using System.Runtime.CompilerServices;

namespace IGApi.Common
{
    internal class LogFormat
    {
        private string _logFormat;

        private LogFormat(string logFormat)
        {
            _logFormat = logFormat;
        }

        public override string ToString()
        {
            return _logFormat;
        }

        public static bool operator ==(LogFormat left, LogFormat right)
        {
            return (left._logFormat == right._logFormat);
        }

        public static bool operator !=(LogFormat left, LogFormat right)
        {
            return (left._logFormat != right._logFormat);
        }

        public static LogFormat FormatOneColumns = new("{0, -50} {1, 150}");
        public static LogFormat FormatTwoColumns = new("{0, -50} {1, 75} {2, 75}");
        public static LogFormat FormatThreeColumns = new("{0, -50} {1, 50} {2, 50} {3, 50}");
        public static LogFormat FormatFourColumns = new("{0, -50} {1, 37} {2, 37} {3, 37} {4, 37}");
    }


    internal static class Log
    {
        private static string? isLockedByCaller;

        internal static string[] Messages(params string[] messages) { return messages; }

        /// <summary>
        /// Allows to get exclusive rights to write to console output stream.
        /// </summary>
        /// <param name="caller"></param>
        internal static void Lock([CallerMemberName] string? caller = null)
        {
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
            Log.WriteLog(Messages(""), caller);
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

        internal static void WriteLog(LogFormat logFormat, string[] messages, [CallerMemberName] string? caller = null)
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
            WriteLog(Messages(ex.ToString()), caller);
        }
    }
}