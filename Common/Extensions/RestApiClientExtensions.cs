using System.Diagnostics.CodeAnalysis;
using IGApi;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using IGApi.Common.Extensions;
using System.Runtime.CompilerServices;

namespace IGApi
{
    using static Log;
    public static partial class RestApiClientExtensions
    {
        public static IgResponse<T>? UseManagedCall<T>(this Task<IgResponse<T>> callTask, [CallerMemberName] string? caller = null)
        {
            IgResponse<T>? response = null;

            int retryCount = ApiEngine.AllowedApiCallsPerMinute;

            bool retry = true;

            while (retry)
            {
                response = Task.Run(async () => await callTask).Result;

                _ = response ?? throw new RestCallNullReferenceException();

                if (response)
                    retry = false;
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && retryCount >= 0)    
                {
                    WriteException(new Exception("Possible temporary reach of api call limit. Retrying call."));

                    Utility.WaitFor(1000 * ApiEngine.CycleTime);

                    retry = true;
                }
                else
                {
                    retry = false;
                    throw new RestCallHttpRequestException(response.StatusCode, caller);
                }

                retryCount--;
            }
            return response;
        }
    }
}
