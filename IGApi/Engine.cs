using System.Collections.Specialized;
using System.Configuration;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.Model;
using IGWebApiClient;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private static readonly Lazy<ApiEngine> lazy =
            new(() => new ApiEngine());

        public static ApiEngine Instance => lazy.Value;

        private CancellationTokenSource _cancellationTokenSource;

        public IgRestApiClient IGRestApiClient => _iGRestApiClient;

        private readonly IgRestApiClient _iGRestApiClient;

        private readonly IGStreamingApiClient _iGStreamApiClient;

        public static readonly int AllowedApiCallsPerMinute = Common.Configuration.GetAllowedApiCallsPerMinute();

        public static readonly int CycleTime = 60 / AllowedApiCallsPerMinute;

        private Task _apiRequestQueueEngineTask;

        private Task _setIsAliveTask;

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

            StreamingTradeDetailsInit();
        }

        public void Start()
        {
            Login();

            if (_cancellationTokenSource is not null)
                _cancellationTokenSource.Dispose();

            _cancellationTokenSource = new CancellationTokenSource();

            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _apiRequestQueueEngineTask = Task.Factory.StartNew(() => RequestQueue.RequestQueueEngine.Start(cancellationToken));

            _setIsAliveTask = Task.Factory.StartNew(() =>
                {
                    bool hasInitializedReported = false;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (RequestQueue.RequestQueueEngine.IsInitialized)
                        {
                            if (!hasInitializedReported)
                            {
                                // ASCII Art: https://patorjk.com/software/taag/#p=display&f=ANSI%20Regular&t=Initialization%20done!
                                WriteLog();
                                WriteLog(Messages("██ ███    ██ ██ ████████ ██  █████  ██      ██ ███████  █████  ████████ ██  ██████  ███    ██     ██████   ██████  ███    ██ ███████ ██"));
                                WriteLog(Messages("██ ████   ██ ██    ██    ██ ██   ██ ██      ██    ███  ██   ██    ██    ██ ██    ██ ████   ██     ██   ██ ██    ██ ████   ██ ██      ██"));
                                WriteLog(Messages("██ ██ ██  ██ ██    ██    ██ ███████ ██      ██   ███   ███████    ██    ██ ██    ██ ██ ██  ██     ██   ██ ██    ██ ██ ██  ██ █████   ██"));
                                WriteLog(Messages("██ ██  ██ ██ ██    ██    ██ ██   ██ ██      ██  ███    ██   ██    ██    ██ ██    ██ ██  ██ ██     ██   ██ ██    ██ ██  ██ ██ ██        "));
                                WriteLog(Messages("██ ██   ████ ██    ██    ██ ██   ██ ███████ ██ ███████ ██   ██    ██    ██  ██████  ██   ████     ██████   ██████  ██   ████ ███████ ██"));
                                WriteLog();
                                WriteLog(Messages("Ready to receive requests..."));

                                hasInitializedReported = true;
                            }
                            ApiEngineStatus.SetIsAlive();
                        }

                        Utility.WaitFor(100);
                    }

                    if (cancellationToken.IsCancellationRequested)
                        WriteLog(Messages("Cancellation request received. Updating alive status has stopped."));
                }
                , cancellationToken
                );
        }

        public void Stop()
        {
            WriteLog(Messages("Cancelling subscribed threads."));
            _cancellationTokenSource.Cancel();

            //  Allow the ApiQueue to finish its tasks.
            _apiRequestQueueEngineTask.Wait();
            _setIsAliveTask.Wait();

            WriteLog(Messages("All subscribed threads have stopped."));

            Logout();
        }

        public void Restart()
        {
            Stop();

            Start();
        }

    }
}
