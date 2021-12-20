namespace IGApi.Common.Extensions
{
    using static Log;
    internal static partial class Extensions
    {
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await Task.Run(task.Start);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }
    }
}
