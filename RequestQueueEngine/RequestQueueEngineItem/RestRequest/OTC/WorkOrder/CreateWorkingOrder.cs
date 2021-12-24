using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? CreateWorkingOrderCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void CreateWorkingOrder()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                dto.endpoint.workingorders.create.v2.CreateWorkingOrderRequest createWorkingOrderRequest = JsonConvert.DeserializeObject<dto.endpoint.workingorders.create.v2.CreateWorkingOrderRequest>(request);

                var response = _apiEngine.IGRestApiClient.createWorkingOrderV2(createWorkingOrderRequest).UseManagedCall();

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
                QueueItemComplete(CreateWorkingOrderCompleted);
            }
        }
    }
}