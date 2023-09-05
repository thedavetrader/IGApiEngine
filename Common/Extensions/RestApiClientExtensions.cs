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
        public static IgResponse<T> UseManagedCall<T>(this Task<IgResponse<T>> callTask, [CallerMemberName] string? caller = null)
        {
            try
            {
                IgResponse<T>? response = null;

                int retryCount = ApiEngine.AllowedApiCallsPerMinute;

                bool retry = true;

                while (retry && retryCount > 0)
                {
                    response = Task.Run(async () => await callTask).Result;

                    _ = response ?? throw new RestCallNullReferenceException();

                    if (response)
                        retry = false;
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && retryCount >= 0)
                        retry = false;
                    //{
                    //    WriteException(new Exception($"Possible temporary reach of api call limit. Retrying call \"{caller}\""));

                    //    Utility.WaitFor((60 / ApiEngine.AllowedApiCallsPerMinute) * 60 * 1000);

                    //    retry = true;
                    //}
                    else
                    {
                        WriteException(new Exception($"Response failed. Statuscode = {response.StatusCode}. Retrying call \"{caller}\""));

                        Utility.WaitFor(10000 * ApiEngine.CycleTime);

                        retry = true;
                    }

                    retryCount--;
                }
                if (response is not null)
                    return response;
                else
                    throw new RestCallHttpResponseNullException();
            }
            catch (RestCallHttpResponseNullException ex)
            {
                WriteException(ex);
                Environment.Exit(0);    // Force exit program and depend on batch to restart. This will
                                        // make sure no other process, call or api requests hangs.
                return null;
            }
        }
    }
}
