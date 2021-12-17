using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
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
                    {
                        if (!response)
                            throw new RestCallHttpRequestException(nameof(EditPosition), response.StatusCode);
                        else
                            checkConfirmationReceived(response.Response.dealReference);
                    }
                    else
                        throw new RestCallNullReferenceException(nameof(EditPosition));
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }

        }
    }
}