using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? ClosePositionCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void ClosePosition()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                dto.endpoint.positions.close.v1.ClosePositionRequest closePositionRequest = JsonConvert.DeserializeObject<dto.endpoint.positions.close.v1.ClosePositionRequest>(request);

                var response = _apiEngine.IGRestApiClient.closePosition(closePositionRequest).UseManagedCall();

                if (response is not null)
                    checkConfirmationReceived(response.Response.dealReference);
                else
                    throw new RestCallNullReferenceException();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(ClosePositionCompleted);
            }
        }
    }
}