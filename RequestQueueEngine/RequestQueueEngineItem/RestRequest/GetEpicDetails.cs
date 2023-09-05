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

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetEpicDetails()
        {
            bool finalize = true;
            const int splitSize = 50;

            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                var request = ApiRequestQueueItem.Parameters;

                if (request is not null)
                {
                    var jsonEpicArray = JArray.Parse(request).ToList();
                    var split = jsonEpicArray.Count > splitSize;

                    if (!split)
                    {
                        finalize = true;

                        var epicString = String.Join(",", jsonEpicArray.Select(item => ((JObject)item).GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>()));

                        var response = _apiEngine.IGRestApiClient.marketDetailsMulti(epicString).UseManagedCall();

                        SyncToDbEpicDetails(response);
                    }
                    else
                    {
                        finalize = false;

                        foreach (var jsonEpicArraySplitList in jsonEpicArray.Split(splitSize))
                        {
                            var parameters = jsonEpicArraySplitList.Select(item => new { epic = ((JObject)item).GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>() }).Distinct();

                            if (parameters.Any())
                            {
                                var childGuid = Guid.NewGuid();

                                string? jsonParameters = null;

                                jsonParameters = JsonConvert.SerializeObject(
                                    parameters,
                                    Formatting.None);

                                if (jsonParameters is not null)
                                {
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

                                                if (!apiDbContext.ApiRequestQueueItems.Any(a => a.ParentGuid == ApiRequestQueueItem.Guid))
                                                    QueueItemComplete(GetEpicDetailsCompleted);

                                                RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;
                                            }
                                        }
                                    }
                                }

                                RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;    // Prevent event is subscribed more then once.
                                RequestQueueEngineItem.GetEpicDetailsCompleted += GetEpicDetailsCompleted;

                                RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetEpicDetails), executeAsap: false, isRecurrent: false, guid: childGuid, parentGuid: ApiRequestQueueItem.Guid, parameters: jsonParameters, cancellationToken: _cancellationToken);
                            }
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

            void SyncToDbEpicDetails(IgResponse<MarketDetailsListResponse> response)
            {
                try
                {
                    if (response.Response is not null)
                    {
                        ApiDbContext apiDbContext = new();

                        response.Response.marketDetails.ForEach(MarketDetail =>
                        {
                            if (MarketDetail is not null)
                                apiDbContext.SaveEpicDetail(MarketDetail.instrument, MarketDetail.snapshot, MarketDetail.dealingRules);
                        });
                        Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
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