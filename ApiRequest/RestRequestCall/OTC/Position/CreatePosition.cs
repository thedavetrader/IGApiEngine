using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public void CreatePosition(string positionRequest)
        {
            try
            {
                dto.endpoint.positions.create.otc.v2.CreatePositionRequest createPositionRequest = JsonConvert.DeserializeObject<dto.endpoint.positions.create.otc.v2.CreatePositionRequest>(positionRequest);

                var response = _apiEngine.IGRestApiClient.createPositionV2(createPositionRequest).UseManagedCall();

                if (response is not null)
                {
                    if (!response)
                        throw new RestCallHttpRequestException(nameof(CreatePosition), response.StatusCode);
                    else
                        checkConfirmationReceived(response.Response.dealReference);
                }
                else
                    throw new RestCallNullReferenceException(nameof(CreatePosition));
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }

        }
    }
}