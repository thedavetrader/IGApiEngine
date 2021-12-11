using System.Collections.Specialized;
using System.Configuration;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        private static readonly Lazy<ApiEngine> lazy =
            new(() => new ApiEngine());

        public static ApiEngine Instance => lazy.Value;

        public IgRestApiClient IGRestApiClient => _iGRestApiClient;

        private readonly IgRestApiClient _iGRestApiClient;

        private readonly IGStreamingApiClient _iGStreamApiClient;

        public static readonly int AllowedApiCallsPerMinute = Common.Configuration.GetAllowedApiCallsPerMinute();
        public static readonly int CycleTime = 60 / AllowedApiCallsPerMinute;

        public ApiEngine()
        {
            if (ConfigurationManager.GetSection("IgWebApiConnection") is NameValueCollection igWebApiConnectionConfig)
            {
                _env = igWebApiConnectionConfig["environment"];
                _userName = igWebApiConnectionConfig["username"];
                _password = igWebApiConnectionConfig["password"];
                _apiKey = igWebApiConnectionConfig["apikey"];

                if (!String.IsNullOrEmpty(_env) &&
                    !String.IsNullOrEmpty(_userName) &&
                    !String.IsNullOrEmpty(_password) &&
                    !String.IsNullOrEmpty(_apiKey))
                {
                    _iGRestApiClient = new IgRestApiClient(_env, (SmartDispatcher)SmartDispatcher.GetInstance());
                    _iGStreamApiClient = new IGStreamingApiClient();
                }
                else
                {
                    throw new InvalidOperationException("Empty environment setting found for IgWebApiConnection. Make sure the environment are set correctly. Environment settings: environment, username, password, apikey");
                }
            }
            else
            {
                throw new InvalidOperationException("No environment settings found for IgWebApiConnection. Make sure the referencing project has the App.config file with environment settings. You can use App.config from this project as template.");
            }

            StreamingTickDataInit();
            StreamingTradeDataInit();

            Task.Run(()=> RestRequest.QueueEngine.Start());

        }
    }
}
