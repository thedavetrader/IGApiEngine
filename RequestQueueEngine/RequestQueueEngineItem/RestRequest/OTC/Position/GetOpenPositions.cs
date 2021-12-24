using IGApi.Common;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetOpenPositionsCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void GetOpenPositions()
        {
            try
            {
                var response = _apiEngine.IGRestApiClient.getOTCOpenPositionsV2().UseManagedCall(); 

                if (response is not null)
                {
                    using ApiDbContext apiDbContext = new();
                    _ = apiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(apiDbContext.OpenPositions));
                    _ = apiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicTicks));

                    var currentApiOpenPositions = response.Response.positions;

                    RemoveObsoleteEpicTicks(apiDbContext);

                    SyncToDbOpenPositions(_currentAccountId, apiDbContext, currentApiOpenPositions);

                    SyncToEpicStreamList(currentApiOpenPositions);

                    var parameters = currentApiOpenPositions.Select(s => new { s.market.epic }).Distinct();

                    if (parameters.Any())
                    {
                        string? jsonParameters = null;

                        jsonParameters = JsonConvert.SerializeObject(
                            parameters,
                            Formatting.None);

                        RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetEpicDetails), true, false, Guid.NewGuid(), null, jsonParameters);
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
                QueueItemComplete(GetOpenPositionsCompleted);
            }

            static void SyncToDbOpenPositions(string currentAccountId, ApiDbContext apiDbContext, List<dto.endpoint.positions.get.otc.v2.OpenPosition> currentApiOpenPositions)
            {
                if (currentApiOpenPositions.Any())
                {
                    //  Upsert position response.
                    currentApiOpenPositions.ForEach(openPosition =>
                    {
                        if (openPosition is not null)
                        {
                            // Upsert OpenPosition
                            apiDbContext.SaveOpenPosition(openPosition.position, currentAccountId, openPosition.market.epic);

                            // Save market data (only Epics where streamingpricesavailable.)
                            if (ApiEngine.EpicStreamPriceAvailableCheck(openPosition.market.epic))
                                apiDbContext.SaveEpicTick(openPosition.market);
                        }
                    });
                }

                //  Remove closed positions (no longer existing).
                apiDbContext.OpenPositions.RemoveRange(
                    apiDbContext.OpenPositions
                        .Where(w => w.AccountId == currentAccountId).ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !currentApiOpenPositions.Any(b => b.position.dealId == a.DealId)));

                Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
            }

            void SyncToEpicStreamList(List<dto.endpoint.positions.get.otc.v2.OpenPosition> currentApiOpenPositions)
            {
                if (currentApiOpenPositions.Any())
                {
                    var epicStreamListItems = currentApiOpenPositions
                        .Where(w => w.market.streamingPricesAvailable)
                        .Select(s => new EpicStreamListItem(s.market.epic, EpicStreamListItem.EpicStreamListItemSource.SourceOpenPositions))
                        .Distinct()
                        .ToList();

                    _apiEngine.SyncEpicStreamListItems(epicStreamListItems, EpicStreamListItem.EpicStreamListItemSource.SourceOpenPositions);
                }
            }
        }
    }
}