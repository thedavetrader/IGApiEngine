using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using IGApi.Common.Extensions;
using Newtonsoft.Json;
using dto.endpoint.prices.v3;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetPriceCompleted;

        private class GetPriceRequest
        {
            public string Epic = String.Empty;
            public string Resolution = String.Empty;
            public DateTime StartDate;
            public DateTime EndDate;
        }

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetPrice()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string? request = ApiRequestQueueItem.Parameters;

                if (request is not null)
                {
                    GetPriceRequest? getPriceRequest = JsonConvert.DeserializeObject<GetPriceRequest>(request);

                    if (getPriceRequest == null)
                        throw new Exception("Failed to Deserialize GetPrice request.");
                    else if
                        (string.IsNullOrEmpty(getPriceRequest.Epic) || string.IsNullOrEmpty(getPriceRequest.Resolution))
                        throw new Exception("The GetPrice request does not contain a valid Epic and/or Resolution.");

                    var response = _apiEngine.IGRestApiClient.priceSearchByDateV3(
                        getPriceRequest.Epic,
                        getPriceRequest.Resolution,
                        //ATTENTION: Uses local time for filtering dates.
                        // The filterdatetime is applied to the startdatetime of the resolution period.
                        // eg.  filtering from 2023-03-20T00:00:00 to 2023-03-21T00:00:00 with resolution Day
                        //      gets 2 days, one starting at 2023-03-20T00:00:00 and one starting at 2023-03-21T00:00:00
                        getPriceRequest.StartDate.ToIgString(),
                        getPriceRequest.EndDate.ToIgString()
                        ).UseManagedCall();

                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        MarkNotAutorized(getPriceRequest.Epic);
                    }
                    else if (response.Response.prices.Count > 0)
                        SyncToDbPrice(getPriceRequest.Epic, getPriceRequest.Resolution, response);
                    else
                    {
                        WriteException(new Exception($"No price data found for epic {getPriceRequest.Epic}"));
                        MarkTimedOut(getPriceRequest.Epic);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(GetPriceCompleted);
            }

            void MarkTimedOut(string epic)
            {
                ApiDbContext apiDbContext = new();

                var epicDetaill = apiDbContext.EpicDetails.Find(epic);

                if (epicDetaill != null)
                {
                    epicDetaill.IsTimedOut = true;
                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }

            void MarkNotAutorized(string epic)
            {
                ApiDbContext apiDbContext = new();

                var epicDetaill = apiDbContext.EpicDetails.Find(epic);

                if(epicDetaill != null)
                {
                    epicDetaill.IsImportAutorized = false;
                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }

            void SyncToDbPrice(string epic, string resolution, IgResponse<PriceList> response)
            {
                if (response.Response is not null)
                {
                    ApiDbContext apiDbContext = new();

                    apiDbContext.SaveEpicSnapshotAllowance(response.Response.metadata.allowance);

                    response.Response.prices.ForEach(priceSnapshot =>
                        {
                            if (priceSnapshot is not null)
                            {
                                apiDbContext.SaveEpicSnapshot(resolution, epic, priceSnapshot);
                                //TODO: apiDbContext.SaveEpicHistory(price);
                                //TODO: write to db received price points, keep score of datapoints (limit 10.000 / week).
                                /*
                                    --get yesterday candles if not exists
                                        -- at end of week: get remaining day candles
                                        --order by date desc
                                        -- until 15 years back
                                    --on get write received pricepoints to db

                                allowanceExpiry (Number)	The number of seconds till the current allowance period will end and the remaining allowance field is reset
                                remainingAllowance (Number)	The number of data points still available to fetch within the current allowance period
                                totalAllowance (Number)	The number of data points the API key and account combination is allowed to fetch in any given allowance period
                                */
                                //WriteLog(price.closePrice.bid.ToString());
                                //WriteLog(snapshotTimeUTC.ToString());
                            }
                        });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}