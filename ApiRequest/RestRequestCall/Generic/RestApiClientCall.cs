using System.Diagnostics.CodeAnalysis;
using IGApi;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using IGApi.Common.Extensions;

namespace IGApi.RestRequest
{
    //public partial class ApiRequestItem
    public static partial class RestApiClientExtensions
    {
        public static IgResponse<T>? UseManagedCall<T>(this Task<IgResponse<T>> callTask)
        {
            IgResponse<T>? response = null;

            int retryCount = ApiEngine.AllowedApiCallsPerMinute;

            bool retry = true;

            while (retry)
            {
                response = Task.Run(async () => await callTask).Result;

                _ = response ?? throw new RestCallNullReferenceException(nameof(response));

                if (response)
                    retry = false;
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && retryCount >= 0)    
                {
                    Log.WriteException(new Exception("Possible temporary reach of api call limit. Retrying call."), nameof(UseManagedCall));

                    Utility.DelayAction(1000 * ApiEngine.CycleTime);

                    retry = true;
                }
                else
                {
                    retry = false;
                    throw new RestCallHttpRequestException(nameof(UseManagedCall), response.StatusCode);
                }

                retryCount--;
            }
            return response;
        }
    }
}
