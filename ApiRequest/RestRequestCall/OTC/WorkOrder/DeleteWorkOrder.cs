using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        private class CustomDeleteWorkingOrderRequest
        {
            public string? dealId { get; set; }

            public dto.endpoint.workingorders.delete.v1.DeleteWorkingOrderRequest? DeleteWorkingOrderRequest { get; set; }
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
                    {
                        if (!response)
                            throw new RestCallHttpRequestException(nameof(DeleteWorkingOrder), response.StatusCode);
                        else
                            checkConfirmationReceived(response.Response.dealReference);
                    }
                    else
                        throw new RestCallNullReferenceException(nameof(DeleteWorkingOrder));
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(DeleteWorkingOrder));
                throw;
            }

        }
    }
}