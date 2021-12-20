using IGApi.Common;
using IGApi.Model;

namespace IGApi.RequestQueue
{
    using static Log;

    internal static class RequestQueueEngine
    {
        private static DateTime _lastExecutionCycleTimestamp = DateTime.UtcNow;

        internal static void Start(CancellationToken cancellationToken)
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
                    var currentTimestamp = DateTime.UtcNow;
                    bool allowExecution = false;
                    List<RequestQueueItem> queueItemsList = new();
                    RequestQueueItem? restRequestItem = null;

                    using IGApiDbContext iGApiDbContext = new();

                    _ = iGApiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ApiRequestQueueItems));

                    if (iGApiDbContext.ApiRequestQueueItems.Any())
                    {
                        foreach (var apiRequestQueueItem in iGApiDbContext.ApiRequestQueueItems)
                        {
                            queueItemsList.Add(new RequestQueueItem(apiRequestQueueItem));
                        }

                        if (queueItemsList.Any())
                        {
                            restRequestItem = queueItemsList.OrderByDescending(o => o.IsTradingRequest).ThenByDescending(o => o.RestRequestQueueItem.ExecuteAsap).ThenBy(o => o.RestRequestQueueItem.Timestamp).FirstOrDefault();
                        }

                        if (restRequestItem is not null)
                        {
                            if (restRequestItem.IsTradingRequest || !restRequestItem.IsRestRequest)
                            {
                                allowExecution = true;  // Trading request and non rest-calls always have the highest priorty, will be executed immediately and do not affect the IG api call limit.
                            }
                            else if (
                                restRequestItem.IsRestRequest && ApiEngine.Instance.IsLoggedIn &&
                                currentTimestamp.CompareTo(_lastExecutionCycleTimestamp.AddSeconds(_cycleTime)) > 0)
                            {
                                allowExecution = true;
                                _lastExecutionCycleTimestamp = currentTimestamp;
                            }
                            else
                                allowExecution = false;

                            if (allowExecution)
                            {
                                _cycleCount = queueItemsList.Where(w => !w.IsTradingRequest).Count();

                                WriteLog();
                                WriteLog(Messages($"Executing \"{restRequestItem.RestRequestQueueItem.Request}\""));
                                WriteLog(Messages("Queuesize", "Cycletime(*s)"));
                                WriteLog(Messages(new string('_', 40), new string('_', 40)));
                                WriteLog(Messages($"{_cycleCount}", $"{_cycleCount * _cycleTime}"));
                                WriteLog();

                                restRequestItem.Execute();

                                if (restRequestItem.RestRequestQueueItem.IsRecurrent && !restRequestItem.IsTradingRequest && !restRequestItem.RestRequestQueueItem.ExecuteAsap)
                                    restRequestItem.RestRequestQueueItem.Timestamp = currentTimestamp;
                                else
                                    iGApiDbContext.ApiRequestQueueItems.Remove(restRequestItem.RestRequestQueueItem);

                                iGApiDbContext.SaveChangesAsync().Wait();   // Use Wait, preventing the loop from popping the same queue item, while the current is beeing deleted.
                            }
                        }
                    }
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
            using IGApiDbContext iGApiDbContext = new();

            _ = iGApiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ApiRequestQueueItems));
            _ = iGApiDbContext.ConfirmResponses ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ConfirmResponses));
            _ = iGApiDbContext.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.WorkingOrders));
            _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));
            _ = iGApiDbContext.Watchlists ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Watchlists));

            iGApiDbContext.ApiRequestQueueItems.RemoveRange(iGApiDbContext.ApiRequestQueueItems);
            iGApiDbContext.ConfirmResponses.RemoveRange(iGApiDbContext.ConfirmResponses);
            iGApiDbContext.WorkingOrders.RemoveRange(iGApiDbContext.WorkingOrders);
            iGApiDbContext.OpenPositions.RemoveRange(iGApiDbContext.OpenPositions);
            iGApiDbContext.Watchlists.RemoveRange(iGApiDbContext.Watchlists);

            //  Use nonasync to prevent concurrency problems in initalization phase.
            //  (Otherwise the queue could start processing requests and simultanously affect the same entities)
            iGApiDbContext.SaveChanges();
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
            EventHandler? GetAccountDetailsCompleted = null;
            EventHandler? GetOpenPositionsCompleted = null;
            EventHandler? GetWorkingOrdersCompleted = null;
            EventHandler? GetWatchlistsCompleted = null;

            GetAccountDetailsCompleted = delegate (object? sender, EventArgs e) { _getAccountDetailsCompleted = true; RequestQueueItem.GetAccountDetailsCompleted -= GetAccountDetailsCompleted; };
            GetOpenPositionsCompleted = delegate (object? sender, EventArgs e) { _getOpenPositionsCompleted = true; RequestQueueItem.GetOpenPositionsCompleted -= GetOpenPositionsCompleted; };
            GetWorkingOrdersCompleted = delegate (object? sender, EventArgs e) { _getWorkingOrdersCompleted = true; RequestQueueItem.GetWorkingOrdersCompleted -= GetWorkingOrdersCompleted; };
            GetWatchlistsCompleted = delegate (object? sender, EventArgs e) { _getWatchlistsCompleted = true; RequestQueueItem.GetWatchlistsCompleted -= GetWatchlistsCompleted; };

            RequestQueueItem.GetAccountDetailsCompleted += GetAccountDetailsCompleted;
            RequestQueueItem.GetOpenPositionsCompleted += GetOpenPositionsCompleted;
            RequestQueueItem.GetWorkingOrdersCompleted += GetWorkingOrdersCompleted;
            RequestQueueItem.GetWatchlistsCompleted += GetWatchlistsCompleted;

            /*  Essential entities  */
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetAccountDetails), false, true);
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetOpenPositions), false, true);
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetWorkingOrders), false, true);
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetWatchlists), false, true);

            /*  Non-essential entities  */
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetActivityHistory), false, true);
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetTransactionHistory), false, true);
            RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetClientSentiment), false, true);
        }
    }
}