using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using System.Collections.Specialized;
using System.Configuration;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private static readonly Lazy<ApiEngine> lazy =
            new(() => new ApiEngine());

        public static ApiEngine Instance => lazy.Value;

        private CancellationTokenSource? _cancellationTokenSource;

        private CancellationToken _cancellationToken;

        public IgRestApiClient IGRestApiClient => _iGRestApiClient;

        private readonly IgRestApiClient _iGRestApiClient;

        private readonly IGStreamingApiClient _iGStreamApiClient;

        public static readonly int AllowedApiCallsPerMinute = Common.Configuration.GetAllowedApiCallsPerMinute();

        public static readonly int CycleTime = 60 / AllowedApiCallsPerMinute;

        private Task? _apiRequestQueueEngineTask;

        private Task? _setIsAliveTask;

        public ApiEngine()
        {
            //if (ConfigurationManager.GetSection("IgWebApiConnection") is NameValueCollection igWebApiConnectionConfig)
            if (ConfigurationManager.GetSection("IgWebApiLiveConnection") is NameValueCollection igWebApiConnectionConfig)
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
            try
            {
                Login();

                if (_cancellationTokenSource is not null)
                    _cancellationTokenSource.Dispose();

                _cancellationTokenSource = new CancellationTokenSource();

                _cancellationToken = _cancellationTokenSource.Token;

                _apiRequestQueueEngineTask = Task.Factory.StartNew(() => RequestQueue.RequestQueueEngine.StartListening(_cancellationToken));

                _setIsAliveTask = Task.Factory.StartNew(() =>
                    {
                        bool hasInitializedReported = false;

                        while (!_cancellationToken.IsCancellationRequested)
                        {
                            if (RequestQueue.RequestQueueEngine.IsInitialized)
                            {
                                if (!hasInitializedReported)
                                {
                                    // ASCII Art: https://patorjk.com/software/taag/#p=display&f=ANSI%20Regular&t=Initialization%20done!

                                    Log.Lock();

                                    Console.WriteLine();
                                    Console.WriteLine(string.Format("{0, 15}{1, 140}{2,15}", "", "██ ███    ██ ██ ████████ ██  █████  ██      ██ ███████  █████  ████████ ██  ██████  ███    ██     ██████   ██████  ███    ██ ███████ ██", ""));
                                    Console.WriteLine(string.Format("{0, 15}{1, 140}{2,15}", "", "██ ████   ██ ██    ██    ██ ██   ██ ██      ██    ███  ██   ██    ██    ██ ██    ██ ████   ██     ██   ██ ██    ██ ████   ██ ██      ██", ""));
                                    Console.WriteLine(string.Format("{0, 15}{1, 140}{2,15}", "", "██ ██ ██  ██ ██    ██    ██ ███████ ██      ██   ███   ███████    ██    ██ ██    ██ ██ ██  ██     ██   ██ ██    ██ ██ ██  ██ █████   ██", ""));
                                    Console.WriteLine(string.Format("{0, 15}{1, 140}{2,15}", "", "██ ██  ██ ██ ██    ██    ██ ██   ██ ██      ██  ███    ██   ██    ██    ██ ██    ██ ██  ██ ██     ██   ██ ██    ██ ██  ██ ██ ██        ", ""));
                                    Console.WriteLine(string.Format("{0, 15}{1, 140}{2,15}", "", "██ ██   ████ ██    ██    ██ ██   ██ ███████ ██ ███████ ██   ██    ██    ██  ██████  ██   ████     ██████   ██████  ██   ████ ███████ ██", ""));
                                    Console.WriteLine();
                                    WriteLog(Columns("Ready to receive requests..."));
                                    WriteLog();

                                    Log.UnLock();

                                    hasInitializedReported = true;
                                }
                                ApiEngineStatus.SetIsAlive(DateTime.UtcNow);
                            }

                            Utility.WaitFor(500);
                        }

                        if (_cancellationToken.IsCancellationRequested)
                            WriteLog(Columns("Cancellation request received. Updating alive status has stopped."));
                    }
                    , _cancellationToken
                    );
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                Log.UnLock();
            }
        }

        public void Stop()
        {
            // We just exit the program, which will automatically unsbscribe further threads.
            //  TODO: Not the most elegant implementation, this relies on a batch program that 
            //  restarts the whole program again in case of a Exception, Crash or deliberate Stop.
            //  But it is the most reliable for now.
            Environment.Exit(0);

            //  Alternative implementation, more elegant, but in reality not the most reliable
            //  in case of some unhandled exceptions:
            
            //Logout();

            //WriteLog(Columns("Cancelling subscribed threads."));
            //if(_cancellationTokenSource is not null) _cancellationTokenSource.Cancel();

            ////  Allow the ApiQueue to finish its tasks.
            //if(_apiRequestQueueEngineTask is not null) _apiRequestQueueEngineTask.Wait();
            //if(_setIsAliveTask is not null) _setIsAliveTask.Wait();
            
            //RequestQueue.RequestQueueEngine.ResetInitialization();

            //WriteLog(Columns("All subscribed processes have stopped."));
        }

        public void Restart()
        {
            throw new NotImplementedException();

            //Stop();

            //Start();
        }

    }
}
