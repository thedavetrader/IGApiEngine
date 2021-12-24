using IGApi.Common;
using IGApi.Model;

namespace IGApi.RequestQueue
{
    using static Log;

    internal static class RequestQueueEngine
    {
        private static DateTime _lastExecutionCycleTimestamp = DateTime.UtcNow;

        private const int heartBeat = 10; //TODO: Make heartbeat configurable

        internal static void StartListening(CancellationToken cancellationToken)
        {
            int _cycleTime = 60 / ApiEngine.AllowedApiCallsPerMinute;
            int _cycleCount;

            InitCleanVolatileTables();
            InitApiRequestQueueItems();

            WriteLog(Messages("QueueEngine starts listening for requests."));

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var dequeueTimestamp = DateTime.UtcNow;
                    bool allowExecution = false;

                    using ApiDbContext apiDbContext = new();

                    _ = apiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiRequestQueueItems));

                    if (apiDbContext.ApiRequestQueueItems.Any())
                    {
                        var queueItemsList = apiDbContext.ApiRequestQueueItems.Where(w => !w.IsRunning).Select(item => new RequestQueueEngineItem(item, dequeueTimestamp)).ToList();

                        if (queueItemsList.Any())
                        {
                            var restRequestItem = queueItemsList.OrderByDescending(o => o.IsTradingRequest).ThenByDescending(o => o.ApiRequestQueueItem.ExecuteAsap).ThenBy(o => o.ApiRequestQueueItem.Timestamp).FirstOrDefault();

                            if (restRequestItem is not null)
                            {
                                if (restRequestItem.IsTradingRequest || !restRequestItem.IsRestRequest)
                                {
                                    allowExecution = true;  // Trading request and non rest-calls always have the highest priorty, will be executed immediately and do not affect the IG api call limit.
                                }
                                else if (
                                    restRequestItem.IsRestRequest && ApiEngine.Instance.IsLoggedIn &&
                                    dequeueTimestamp.CompareTo(_lastExecutionCycleTimestamp.AddSeconds(_cycleTime)) > 0)
                                {
                                    allowExecution = true;
                                    _lastExecutionCycleTimestamp = dequeueTimestamp;
                                }
                                else
                                    allowExecution = false;

                                if (allowExecution)
                                {
                                    // TODO:
                                    // when starting app, remove all restrequest
                                    //  set non restrequest runnin = false (so they can be picked up asap).

                                    restRequestItem.ApiRequestQueueItem.IsRunning = true;
                                    apiDbContext.SaveChangesAsync().Wait();

                                    _cycleCount = queueItemsList.Where(w => !w.IsTradingRequest).Count();

                                    WriteLog();
                                    WriteLog(Messages($"Executing \"{restRequestItem.ApiRequestQueueItem.Request}\""));
                                    WriteLog(Messages("Queuesize", "Cycletime(*s)"));
                                    WriteLog(Messages(new string('_', 40), new string('_', 40)));
                                    WriteLog(Messages($"{_cycleCount}", $"{_cycleCount * _cycleTime}"));
                                    WriteLog();

                                    restRequestItem.Execute();
                                    //RestQueueItemComplete(apiDbContext, restRequestItem);   // Use Wait, preventing the loop from popping the same queue item, while the current is beeing deleted.
                                }
                            }
                        }
                    }

                    Utility.WaitFor(heartBeat);    // Give the cpu a break.
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    throw;
                }
            }

            if (cancellationToken.IsCancellationRequested)
                WriteLog(Messages("Cancellation request received. QueueEngine has stopped listening for requests."));
        }

        /// <summary>
        /// Open positions and working orders could be changed during the time the API was not running. 
        /// Therefor assume the current state invalid and regard them as obsolete and to be removed.
        /// </summary>
        /// <exception cref="DBContextNullReferenceException"></exception>
        private static void InitCleanVolatileTables()
        {
            using ApiDbContext apiDbContext = new();

            _ = apiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiRequestQueueItems));
            _ = apiDbContext.ConfirmResponses ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ConfirmResponses));
            _ = apiDbContext.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(apiDbContext.WorkingOrders));
            _ = apiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(apiDbContext.OpenPositions));
            _ = apiDbContext.Watchlists ?? throw new DBContextNullReferenceException(nameof(apiDbContext.Watchlists));

            apiDbContext.ApiRequestQueueItems.RemoveRange(apiDbContext.ApiRequestQueueItems.ToList().Where(r => new RequestQueueEngineItem(r, DateTime.Now).IsRestRequest)); apiDbContext.SaveChanges(); // Only remove restrequests.
            apiDbContext.ConfirmResponses.RemoveRange(apiDbContext.ConfirmResponses); apiDbContext.SaveChanges();
            apiDbContext.WorkingOrders.RemoveRange(apiDbContext.WorkingOrders); apiDbContext.SaveChanges();
            apiDbContext.OpenPositions.RemoveRange(apiDbContext.OpenPositions); apiDbContext.SaveChanges();
            apiDbContext.Watchlists.RemoveRange(apiDbContext.Watchlists); apiDbContext.SaveChanges();


        }

        /*  Status of essential entity initialization   */
        private static bool _getAccountDetailsCompleted = false;
        private static bool _getOpenPositionsCompleted = false;
        private static bool _getWorkingOrdersCompleted = false;
        private static bool _getWatchlistsCompleted = false;

        public static bool IsInitialized
        {
            get
            {
                return
                    _getAccountDetailsCompleted &&
                    _getOpenPositionsCompleted &&
                    _getWorkingOrdersCompleted &&
                    _getWatchlistsCompleted;
            }
        }

        /// <summary>
        /// Make sure essential details are queued to recurrently refresh.
        /// </summary>
        private static void InitApiRequestQueueItems()
        {
            static void GetAccountDetailsCompleted(object? sender, EventArgs e) { _getAccountDetailsCompleted = true; RequestQueueEngineItem.GetAccountDetailsCompleted -= GetAccountDetailsCompleted; }
            static void GetOpenPositionsCompleted(object? sender, EventArgs e) { _getOpenPositionsCompleted = true; RequestQueueEngineItem.GetOpenPositionsCompleted -= GetOpenPositionsCompleted; }
            static void GetWorkingOrdersCompleted(object? sender, EventArgs e) { _getWorkingOrdersCompleted = true; RequestQueueEngineItem.GetWorkingOrdersCompleted -= GetWorkingOrdersCompleted; }
            static void GetWatchlistsCompleted(object? sender, EventArgs e) { _getWatchlistsCompleted = true; RequestQueueEngineItem.GetWatchlistsCompleted -= GetWatchlistsCompleted; }

            RequestQueueEngineItem.GetAccountDetailsCompleted += GetAccountDetailsCompleted;
            RequestQueueEngineItem.GetOpenPositionsCompleted += GetOpenPositionsCompleted;
            RequestQueueEngineItem.GetWorkingOrdersCompleted += GetWorkingOrdersCompleted;
            RequestQueueEngineItem.GetWatchlistsCompleted += GetWatchlistsCompleted;

            /*  Essential entities  */
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetAccountDetails), false, true, Guid.NewGuid());
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetOpenPositions), false, true, Guid.NewGuid());
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetWorkingOrders), false, true, Guid.NewGuid());
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetWatchlists), false, false, Guid.NewGuid());   // Init only, not recurrent. One-way, api point of view. Only update watchlists when changes are initiated by api.

            /*  Non-essential entities  */
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetActivityHistory), false, true, Guid.NewGuid());
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetTransactionHistory), false, true, Guid.NewGuid());
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetClientSentiment), false, true, Guid.NewGuid());
        }
    }
}