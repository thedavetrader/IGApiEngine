using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? EditWorkingOrderCompleted;
        private class CustomEditWorkingOrderRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.workingorders.edit.v2.EditWorkingOrderRequest? EditWorkingOrderRequest { get; set; }
        }

        public void EditWorkingOrder(string WorkingOrderRequest)
        {
            try
            {
                CustomEditWorkingOrderRequest EditWorkingOrderRequest = JsonConvert.DeserializeObject<CustomEditWorkingOrderRequest>(WorkingOrderRequest);

                if (!string.IsNullOrEmpty(EditWorkingOrderRequest.dealId))    //EditWorkingOrderRequest has no members, so is allowed to be null.
                {
                    var response = _apiEngine.IGRestApiClient.editWorkingOrderV2(EditWorkingOrderRequest.dealId, EditWorkingOrderRequest.EditWorkingOrderRequest).UseManagedCall();

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
                EditWorkingOrderCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}