using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public void CreateWorkingOrder(string WorkingOrderRequest)
        {
            try
            {
                dto.endpoint.workingorders.create.v2.CreateWorkingOrderRequest createWorkingOrderRequest = JsonConvert.DeserializeObject<dto.endpoint.workingorders.create.v2.CreateWorkingOrderRequest>(WorkingOrderRequest);

                var response = _apiEngine.IGRestApiClient.createWorkingOrderV2(createWorkingOrderRequest).UseManagedCall();

                if (response is not null)
                {
                    if (!response)
                        throw new RestCallHttpRequestException(nameof(CreateWorkingOrder), response.StatusCode);
                    else
                        checkConfirmationReceived(response.Response.dealReference);
                }
                else
                    throw new RestCallNullReferenceException(nameof(CreateWorkingOrder));
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(CreateWorkingOrder));
                throw;
            }

        }
    }
}