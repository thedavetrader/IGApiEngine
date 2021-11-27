using System.Collections.Specialized;
using System.Configuration;
using IGApi.Common;
using IGApiEngine.IGRestApiQueueEngine;
using IGWebApiClient;

namespace IGApi
{
    public sealed partial class IGApiEngine
    {
        private static readonly Lazy<IGApiEngine> lazy =
            new(() => new IGApiEngine());

        public static IGApiEngine Instance => lazy.Value;

        public IgRestApiClient IGRestApiClient => _iGRestApiClient;

        private readonly IgRestApiClient _iGRestApiClient;

        private readonly IGStreamingApiClient _iGStreamApiClient;

        public IGApiEngine()
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
                    _iGRestApiClient = new IgRestApiClient(_env, (SmartDispatcher)SmartDispatcher.getInstance());
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

            Task.Run(()=> IGRestApiQueueEngine.Start());

        }
    }
}
