using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IGApi.Model;
using IGApiEngine.Common;

namespace IGApiEngine.IGRestApiQueueEngine
{
    public partial class RestRequest
    {
        public bool IsTradingRequest;

        public IGRestApiQueue IGRestApiQueueItem;

        private readonly Task _restRequestCallTask;

        public RestRequest([NotNullAttribute] IGRestApiQueue iGRestApiQueueItem)
        {
            IGRestApiQueueItem = iGRestApiQueueItem;

            switch (IGRestApiQueueItem.RestRequest)
            {
                case "GetAccountDetails":
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetAccountDetails());
                        break;
                    }
                case "GetOpenPositions":
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = Task.CompletedTask;
                        break;
                    }
                case "CreatePosition":
                    {
                        IsTradingRequest = true;
                        _restRequestCallTask = Task.CompletedTask;
                        break;
                    }
                default:
                    {
                        throw new Exception($"The restrequestcall is invalid. The check constraint on column RestRequest of table IGRestApiQueue is possibly corrupt or missing.");
                    }
            }
        }

        public void Execute()
        {
            _restRequestCallTask.FireAndForget();
        }
    }

    internal static class IGRestApiQueueEngine
    {
        private static DateTime _lastExecutionCycleTimestamp = DateTime.UtcNow;
        private static int _cycleTime;
        private static int _cycleCount;

        internal static void Start()
        {
            if (ConfigurationManager.GetSection("IGRestApiQueueEngine") is NameValueCollection IGRestApiQueueEngine)
            {
                if (!int.TryParse(IGRestApiQueueEngine["IGAllowedApiCallsPerMinute"], out int allowedApiCallsPerMinute))
                {
                    throw new InvalidOperationException("Could not parse the environment setting IGAllowedApiCallsPerMinute. Make sure the environment are set correctly. It should be an integer that represents the amount of api calls allowed to make per minute (typically 30 in live environment).");
                }
                _cycleTime = 60 / allowedApiCallsPerMinute;
            }
            else
            {
                throw new InvalidOperationException("No environment settings found for IGRestApiQueueEngine. Make sure the referencing project has the App.config file with environment settings. You can use App.config from this project as template.");
            }

            Log.WriteLine("IGRestApiQueueEngine is listening for restrequests.");

            while (true)
            {
                var currentTimestamp = DateTime.UtcNow;
                bool allowExecution = false;
                List<RestRequest> queueItemsList = new();
                RestRequest? restRequestItem = null;

                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.IGRestApiQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGRestApiQueue));

                if (iGApiDbContext.IGRestApiQueue.Any())
                {
                    foreach (var iGRestApiQueueItem in iGApiDbContext.IGRestApiQueue)
                    {
                        queueItemsList.Add(new RestRequest(iGRestApiQueueItem));
                    }

                    if (queueItemsList.Any())
                    {
                        restRequestItem = queueItemsList.OrderByDescending(o => o.IsTradingRequest).ThenByDescending(o => o.IGRestApiQueueItem.ExecuteAsap).ThenBy(o => o.IGRestApiQueueItem.Timestamp).FirstOrDefault();
                    }

                    if (restRequestItem is not null)
                    {
                        if (restRequestItem.IsTradingRequest)
                        {
                            allowExecution = true;  // Trading request always have the highest priorty, will be executed immediately and do not affect the IG api call limit.
                        }
                        else if (currentTimestamp.CompareTo(_lastExecutionCycleTimestamp.AddSeconds(_cycleTime)) > 0)
                        {
                            allowExecution = true;
                            _lastExecutionCycleTimestamp = currentTimestamp;
                        }
                        else
                            allowExecution = false;

                        if (allowExecution)
                        {
                            _cycleCount = queueItemsList.Where(w => !w.IsTradingRequest).Count();

                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, "[IGRestApiQueue] {0, -40} {1, -40} {2, -40}"
                                                            , $"Executing \"{restRequestItem.IGRestApiQueueItem.RestRequest}\""
                                                            , $"Current recurring items in queue: { _cycleCount}"
                                                            , $"Current cycletime(*s) per item: {_cycleCount * _cycleTime}"));

                            restRequestItem.Execute();

                            if (restRequestItem.IGRestApiQueueItem.IsRecurrent && !restRequestItem.IsTradingRequest && !restRequestItem.IGRestApiQueueItem.ExecuteAsap)
                                restRequestItem.IGRestApiQueueItem.Timestamp = currentTimestamp;
                            else
                                iGApiDbContext.IGRestApiQueue.Remove(restRequestItem.IGRestApiQueueItem);

                            iGApiDbContext.SaveChangesAsync().Wait();   // Use Wait, preventing the loop from popping the same queue item, while the current is beeing deleted.
                        }
                    }
                }
            }
        }
    }
}