﻿using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using IGApi.Common;
using IGApi.Model;

namespace IGApi.RestRequest
{
    internal static class QueueEngine
    {
        private static DateTime _lastExecutionCycleTimestamp = DateTime.UtcNow;
        
        internal static void Start()
        {
            int _cycleTime = 60 / ApiEngine.AllowedApiCallsPerMinute;
            int _cycleCount;

            InitResQueueItems();

            Log.WriteLine("IGRestRequestQueueEngine is listening for restrequests.");

            while (true)
            {
                var currentTimestamp = DateTime.UtcNow;
                bool allowExecution = false;
                List<RestRequest> queueItemsList = new();
                RestRequest? restRequestItem = null;

                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

                if (iGApiDbContext.RestRequestQueue.Any())
                {
                    foreach (var IGRestRequestQueueItem in iGApiDbContext.RestRequestQueue)
                    {
                        queueItemsList.Add(new RestRequest(IGRestRequestQueueItem));
                    }

                    if (queueItemsList.Any())
                    {
                        restRequestItem = queueItemsList.OrderByDescending(o => o.IsTradingRequest).ThenByDescending(o => o.RestRequestQueueItem.ExecuteAsap).ThenBy(o => o.RestRequestQueueItem.Timestamp).FirstOrDefault();
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

                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[IGRestRequestQueue]", "", "", ""));
                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatTwoColumns, "[IGRestRequestQueue]", $"Executing \"{restRequestItem.RestRequestQueueItem.RestRequest}\""));
                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[IGRestRequestQueue]", "", "", ""));
                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[IGRestRequestQueue]", "", "Queuesize", "Cycletime(*s)"));
                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[IGRestRequestQueue]", "", new string('_', 40), new string('_', 40)));
                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[IGRestRequestQueue]", "", $"{_cycleCount}", $"{_cycleCount * _cycleTime}"));
                            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[IGRestRequestQueue]", "", "", ""));

                            restRequestItem.Execute();

                            if (restRequestItem.RestRequestQueueItem.IsRecurrent && !restRequestItem.IsTradingRequest && !restRequestItem.RestRequestQueueItem.ExecuteAsap)
                                restRequestItem.RestRequestQueueItem.Timestamp = currentTimestamp;
                            else
                                iGApiDbContext.RestRequestQueue.Remove(restRequestItem.RestRequestQueueItem);

                            iGApiDbContext.SaveChangesAsync().Wait();   // Use Wait, preventing the loop from popping the same queue item, while the current is beeing deleted.
                        }
                    }
                }
            }
        }

        private static void InitResQueueItems()
        {
            RestQueueQueueItem.RestQueueGetOpenPositions();
            RestQueueQueueItem.RestQueueGetAccountDetails();
        }
    }
}