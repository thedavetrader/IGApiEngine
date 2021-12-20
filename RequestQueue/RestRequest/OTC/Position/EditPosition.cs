using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? EditPositionCompleted;
        private class CustomEditPositionRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.positions.edit.v2.EditPositionRequest? EditPositionRequest { get; set; }
        }

        public void EditPosition(string positionRequest)
        {
            try
            {
                var editPositionRequest = JsonConvert.DeserializeObject<CustomEditPositionRequest>(positionRequest);

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
                EditPositionCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}