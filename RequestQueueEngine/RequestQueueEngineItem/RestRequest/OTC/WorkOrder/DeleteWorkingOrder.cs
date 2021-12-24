using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? DeleteWorkingOrderCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        private class CustomDeleteWorkingOrderRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.workingorders.delete.v1.DeleteWorkingOrderRequest? DeleteWorkingOrderRequest { get; set; } = new();
        }

        public void DeleteWorkingOrder()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomDeleteWorkingOrderRequest deleteWorkingOrderRequest = JsonConvert.DeserializeObject<CustomDeleteWorkingOrderRequest>(request);

                if (!string.IsNullOrEmpty(deleteWorkingOrderRequest.dealId))    //DeleteWorkingOrderRequest has no members, so is allowed to be null.
                {
                    var response = _apiEngine.IGRestApiClient.deleteWorkingOrder(deleteWorkingOrderRequest.dealId, deleteWorkingOrderRequest.DeleteWorkingOrderRequest).UseManagedCall();

                    if (response is not null)
                        checkConfirmationReceived(response.Response.dealReference);
                    else
                        throw new RestCallNullReferenceException();
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(DeleteWorkingOrderCompleted);
            }
        }
    }
}