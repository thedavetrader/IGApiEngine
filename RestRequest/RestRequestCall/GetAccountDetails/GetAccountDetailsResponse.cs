using dto.endpoint.accountbalance;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        private IgResponse<AccountDetailsResponse>? GetAccountDetailsResponse()
        {
            IGWebApiClient.IgResponse<AccountDetailsResponse>? response = null;

            int retryCount = ApiEngine.AllowedApiCallsPerMinute;

            bool retry = true;

            while (retry)
            {
                response = Task.Run(async () => await _apiEngine.IGRestApiClient.accountBalance()).Result;

                _ = response ?? throw new RestCallNullReferenceException(nameof(response));

                if (response.StatusCode == System.Net.HttpStatusCode.OK) 
                    retry = false;
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && retryCount >= 0)    // possible temporary reach of api call limit.
                {
                    Utility.DelayAction(1000 * ApiEngine.CycleTime);
                    retry = true;
                }
                else
                {
                    retry = false;
                    throw new RestCallHttpRequestException(nameof(GetEpicDetails), response.StatusCode);
                }

                retryCount--;
            }

            return response;

        }
    }
}
