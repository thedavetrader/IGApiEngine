using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? EditWorkingOrderCompleted;
        private class CustomEditWorkingOrderRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.workingorders.edit.v2.EditWorkingOrderRequest? EditWorkingOrderRequest { get; set; }
        }

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void EditWorkingOrder()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomEditWorkingOrderRequest editWorkingOrderRequest = JsonConvert.DeserializeObject<CustomEditWorkingOrderRequest>(request);

                if (!string.IsNullOrEmpty(editWorkingOrderRequest.dealId))    //EditWorkingOrderRequest has no members, so is allowed to be null.
                {
                    var response = _apiEngine.IGRestApiClient.editWorkingOrderV2(editWorkingOrderRequest.dealId, editWorkingOrderRequest.EditWorkingOrderRequest).UseManagedCall();

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
                QueueItemComplete(EditWorkingOrderCompleted);
            }
        }
    }
}