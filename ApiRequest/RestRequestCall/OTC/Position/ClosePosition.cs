using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public void ClosePosition(string positionRequest)
        {
            try
            {
                dto.endpoint.positions.close.v1.ClosePositionRequest closePositionRequest = JsonConvert.DeserializeObject<dto.endpoint.positions.close.v1.ClosePositionRequest>(positionRequest);

                var response = _apiEngine.IGRestApiClient.closePosition(closePositionRequest).UseManagedCall();

                if (response is not null)
                {
                    if (!response)
                        throw new RestCallHttpRequestException(nameof(ClosePosition), response.StatusCode);
                    else
                        checkConfirmationReceived(response.Response.dealReference);
                }
                else
                    throw new RestCallNullReferenceException(nameof(ClosePosition));
                }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }

        }
    }
}