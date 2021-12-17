using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
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
                    {
                        if (!response)
                            throw new RestCallHttpRequestException(nameof(EditWorkingOrder), response.StatusCode);
                        else
                            checkConfirmationReceived(response.Response.dealReference);
                    }
                    else
                        throw new RestCallNullReferenceException(nameof(EditWorkingOrder));
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(EditWorkingOrder));
                throw;
            }

        }
    }
}