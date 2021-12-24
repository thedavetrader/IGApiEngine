using System;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.Model;
using IGWebApiClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetEpicDetailsCompleted;

        //TODO: GetEpicDetails Make version that refresh epics in epicstreamlist UNION existing epic details on db.
        //      Use restapiqueue to queue the request and split over max limit 50.
        [Obsolete("Make version that recurrently refresh epics in epicstreamlist UNION existing epic details on db + all watchlists. Use restapiqueue to queue the request and split over max limit 50.")]
        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetEpicDetails()
        {
            bool finalize = true;
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                var jsonEpicArray = JArray.Parse(request).ToList();
                string epicString;
                var split = jsonEpicArray.Count > 50;

                if (!split)
                {
                    finalize = true;

                    epicString = String.Join(",", jsonEpicArray.Select(item => ((JObject)item).GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>()));

                    var response = _apiEngine.IGRestApiClient.marketDetailsMulti(epicString).UseManagedCall();

                    SyncToDbEpicDetails(response);
                }
                else
                {
                    finalize = false;

                    foreach (var jsonEpicArraySplitList in jsonEpicArray.Split(50))
                    {
                        var parameters = jsonEpicArraySplitList.Select(item => new { epic = ((JObject)item).GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>() }).Distinct();

                        if (parameters.Any())
                        {
                            var childGuid = Guid.NewGuid();

                            string? jsonParameters = null;

                            jsonParameters = JsonConvert.SerializeObject(
                                parameters,
                                Formatting.None);

                            // Keeping this object hooked until all childs have finished task.
                            void GetEpicDetailsCompleted(object? sender, EventArgs e)
                            {
                                if (sender is not null)
                                {
                                    var senderEngineItem = (RequestQueueEngineItem)sender;

                                    if (senderEngineItem.ApiRequestQueueItem.Guid == childGuid &&
                                        senderEngineItem.ApiRequestQueueItem.ParentGuid == ApiRequestQueueItem.Guid)
                                    {
                                        ApiDbContext apiDbContext = new();

                                        _ = apiDbContext.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiRequestQueueItems));  // critical exception. Should not rise.

                                        if (!apiDbContext.ApiRequestQueueItems.Any(a => a.ParentGuid == ApiRequestQueueItem.Guid))
                                            QueueItemComplete(GetEpicDetailsCompleted);

                                        RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;
                                    }
                                }
                            }

                            RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;    // Prevent event is subscribed more then once.
                            RequestQueueEngineItem.GetEpicDetailsCompleted += GetEpicDetailsCompleted;

                            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetEpicDetails), executeAsap: true, isRecurrent: false, guid: childGuid, parentGuid: ApiRequestQueueItem.Guid, parameters: jsonParameters);
                        }
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
                if (finalize)   // Finalization is done in the eventhandler when this request splits in multiple child requests.
                    QueueItemComplete(GetEpicDetailsCompleted);
            }

            static void SyncToDbEpicDetails(IgResponse<MarketDetailsListResponse>? response)
            {
                try
                {
                    ApiDbContext apiDbContext = new();

                    _ = apiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetails));

                    if (response is not null)
                    {
                        response.Response.marketDetails.ForEach(MarketDetail =>
                        {
                            if (MarketDetail is not null)
                                apiDbContext.SaveEpicDetail(MarketDetail.instrument, MarketDetail.dealingRules);
                        });

                        Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    throw;
                }
            }
        }
    }
}