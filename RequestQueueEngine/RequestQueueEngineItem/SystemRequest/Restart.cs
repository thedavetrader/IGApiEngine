using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? RestartCompleted;

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void Restart()
        {
            try
            {
                ApiEngine.Instance.Restart();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(RestartCompleted);
            }
        }
    }
}