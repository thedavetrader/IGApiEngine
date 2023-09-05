using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? EditPositionCompleted;

        private class CustomEditPositionRequest
        {
            public string? DealId { get; set; }

            public dto.endpoint.positions.edit.v2.EditPositionRequest? EditPositionRequest { get; set; }
        }

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void EditPosition()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                var editPositionRequest = JsonConvert.DeserializeObject<CustomEditPositionRequest>(request);

                if (!string.IsNullOrEmpty(editPositionRequest.DealId) && editPositionRequest.EditPositionRequest is not null)
                {
                    var response = _apiEngine.IGRestApiClient.editPositionV2(editPositionRequest.DealId, editPositionRequest.EditPositionRequest).UseManagedCall();

                    checkConfirmationReceived(response.Response.dealReference);
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