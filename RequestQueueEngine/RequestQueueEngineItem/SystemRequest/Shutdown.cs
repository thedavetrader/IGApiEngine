using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? ShutdownCompleted;

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void Shutdown()
        {
            try
            {
                ApiEngine.Instance.Stop();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(ShutdownCompleted);
            }
        }
    }
}