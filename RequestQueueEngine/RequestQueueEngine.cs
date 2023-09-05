using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using IGApi.Common;
using IGApi.Model;
using Microsoft.EntityFrameworkCore;

namespace IGApi.RequestQueue
{
    using static Log;

    internal static partial class RequestQueueEngine
    {
        private static DateTime _lastExecutionCycleTimestamp = DateTime.UtcNow;
        private static CancellationToken _cancellationToken;

        private const int heartBeat = 16; //TODO: Make heartbeat configurable

        /*  Status of essential entity initialization   */
        private static bool _getAccountDetailsCompleted = false;
        private static bool _getOpenPositionsCompleted = false;
        private static bool _getWorkingOrdersCompleted = false;
        private static bool _getWatchlistsCompleted = false;
        public class RequestListItem
        {
            public string Request { get; }
            public RequestTypeAttribute RequestTypeAttribute { get; }

            public RequestListItem(string request, RequestTypeAttribute? requestTypeAttribute)
            {
                _ = requestTypeAttribute ?? throw new ArgumentNullException(nameof(requestTypeAttribute));

                Request = request;
                RequestTypeAttribute = requestTypeAttribute;
            }
        }

        private static readonly IEnumerable<RequestListItem> ExposedRequests = typeof(RequestQueueEngineItem)
                                                                        .GetMethods()
                                                                        .Where(method => Attribute.GetCustomAttribute(method, typeof(RequestTypeAttribute)) is not null)
                                                                        .Select(method => new RequestListItem(request: method.Name, requestTypeAttribute: (RequestTypeAttribute?)Attribute.GetCustomAttribute(method, typeof(RequestTypeAttribute))));

        internal static void StartListening(CancellationToken cancellationToken)
        {
            int _cycleTime = 60 / ApiEngine.AllowedApiCallsPerMinute;

            _cancellationToken = cancellationToken;

            InitDbObjects();
            InitTables();
            InitApiRequestQueueItems();

            WriteOk(Columns("QueueEngine starts listening for requests."));

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var dequeueTimestamp = DateTime.UtcNow;

                    using ApiDbContext apiDbContext = new();
                    var connectionString = apiDbContext.Database.GetDbConnection().ConnectionString;

                    var isNewCycle = dequeueTimestamp.CompareTo(_lastExecutionCycleTimestamp.AddSeconds(_cycleTime)) > 0;

                    if (isNewCycle)
                        _lastExecutionCycleTimestamp = dequeueTimestamp;

                    var restRequestItem = PopApiRequestQueueItem(connectionString, ExposedRequests, isNewCycle, dequeueTimestamp, _cancellationToken).Result;

                    if (restRequestItem is not null)
                    {
                        if (Common.Configuration.VerboseLog)
                            WriteLog(Columns(
                                $"Executing",
                                $"[{restRequestItem.ApiRequestQueueItem.Request}][{restRequestItem.ApiRequestQueueItem.Guid}]"
                                ));

                        restRequestItem.Execute();
                    }

                    Utility.WaitFor(500);
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    //TODO: Enable auto restart. Current implementation does not login again correctly for datastream objects. Therefor just exit for now.
                    ApiEngine.Instance.Stop();
                    Environment.Exit(0);
                }
            }

            if (cancellationToken.IsCancellationRequested)
                WriteLog(Columns("Cancellation request received. QueueEngine has stopped listening for requests."));

            static async Task<RequestQueueEngineItem?> PopApiRequestQueueItem(string connectionString, IEnumerable<RequestListItem> requests, bool OnCycle, DateTime dequeueTimestamp, CancellationToken cancellationToken)
            {
                if (requests.Any())
                {
                    RequestQueueEngineItem? requestQueueEngineItem = null;

                    using SqlConnection connection = new(connectionString);
                    await connection.OpenAsync(CancellationToken.None);

                    DataTable requestList = new();
                    requestList.Columns.Add("request", typeof(string));
                    requestList.Columns.Add("is_rest_request", typeof(bool));
                    requestList.Columns.Add("is_trading_request", typeof(bool));

                    foreach (RequestListItem request in requests)
                    {
                        requestList.Rows.Add(
                            request.Request,
                            request.RequestTypeAttribute.IsRestRequest,
                            request.RequestTypeAttribute.IsTradingRequest);
                    }

                    SqlCommand command = new("dbo.pop_api_request_queue_item", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlParameter paramRequests = command.Parameters.AddWithValue("@request_list", requestList);
                    paramRequests.SqlDbType = SqlDbType.Structured;
                    paramRequests.TypeName = "request_list";

                    SqlParameter paramOnCycle = command.Parameters.AddWithValue("@on_cycle", OnCycle);
                    paramOnCycle.SqlDbType = SqlDbType.Bit;

                    using (var reader = await command.ExecuteReaderAsync(CancellationToken.None))
                    {
                        if (await reader.ReadAsync(CancellationToken.None))   // Only 1 row expected, so no use of while
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("request")))
                            {
                                ApiRequestQueueItem apiRequestQueueItem = new(
                                    restRequest: reader.GetString("request"),
                                    parameters: reader.IsDBNull(reader.GetOrdinal("parameter")) ? null : reader.GetString("parameter"),
                                    executeAsap: reader.GetBoolean("execute_asap"),
                                    isRecurrent: reader.GetBoolean("is_recurrent"),
                                    guid: reader.GetGuid("guid"),
                                    parentGuid: reader.IsDBNull(reader.GetOrdinal("parent_guid")) ? null : reader.GetGuid("parent_guid"));

                                requestQueueEngineItem = new RequestQueueEngineItem(apiRequestQueueItem, dequeueTimestamp, _cancellationToken);
                            }
                        }
                    }

                    await connection.CloseAsync();

                    return requestQueueEngineItem;
                }
                else
                    return null;
            }
        }
    }
}