using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? DeleteWorkingOrderCompleted;
        private class CustomDeleteWorkingOrderRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.workingorders.delete.v1.DeleteWorkingOrderRequest? DeleteWorkingOrderRequest { get; set; } = new();
        }

        public void DeleteWorkingOrder(string WorkingOrderRequest)
        {
            try
            {
                CustomDeleteWorkingOrderRequest deleteWorkingOrderRequest = JsonConvert.DeserializeObject<CustomDeleteWorkingOrderRequest>(WorkingOrderRequest);

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
                DeleteWorkingOrderCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}