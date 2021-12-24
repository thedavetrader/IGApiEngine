using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? EditPositionCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        private class CustomEditPositionRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.positions.edit.v2.EditPositionRequest? EditPositionRequest { get; set; }
        }

        public void EditPosition()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                var editPositionRequest = JsonConvert.DeserializeObject<CustomEditPositionRequest>(request);

                if (!string.IsNullOrEmpty(editPositionRequest.dealId) && editPositionRequest.EditPositionRequest is not null)
                {
                    var response = _apiEngine.IGRestApiClient.editPositionV2(editPositionRequest.dealId, editPositionRequest.EditPositionRequest).UseManagedCall();

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
                QueueItemComplete(EditPositionCompleted);
            }
        }
    }
}