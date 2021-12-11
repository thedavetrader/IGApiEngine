using IGApi.Common;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        public void GetOpenPositions()
        {
            try
            {
                string? currentAccountId = _apiEngine.LoginSessionInformation?.currentAccountId;
                _ = currentAccountId ?? throw new NullReferenceException(nameof(currentAccountId));

                var response = RestApiClientCall(_apiEngine.IGRestApiClient.getOTCOpenPositionsV2());

                if (response is not null)
                {
                    using IGApiDbContext iGApiDbContext = new();
                    _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));
                    _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

                    var currentApiOpenPositions = response.Response.positions;

                    RemoveObsoleteEpicTicks(iGApiDbContext);

                    SyncToDbOpenPositions(currentAccountId, iGApiDbContext, currentApiOpenPositions);

                    SyncToEpicStreamList(currentApiOpenPositions);

                    var parameters = currentApiOpenPositions.Select(s => new { s.market.epic }).Distinct();

                    if (parameters.Any())
                    {
                        string? jsonParameters = null;

                        jsonParameters = JsonConvert.SerializeObject(
                            parameters,
                            Formatting.None);

                        QueueQueueItem.QueueItem(nameof(RestRequest.GetEpicDetails), true, false, jsonParameters);

                        //TODO: Notify GetTradeActivity
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }

            static void SyncToDbOpenPositions(string? currentAccountId, IGApiDbContext iGApiDbContext, List<dto.endpoint.positions.get.otc.v2.OpenPosition> currentApiOpenPositions)
            {
                if (currentApiOpenPositions.Any())
                {
                    //  Upsert position response.
                    currentApiOpenPositions.ForEach(OpenPosition =>
                    {
                        if (OpenPosition is not null)
                        {
                            // Upsert OpenPosition
                            iGApiDbContext.SaveOpenPosition(OpenPosition.position, currentAccountId, OpenPosition.market.epic);

                            // Save market data (only Epics where streamingpricesavailable.)
                            if (ApiEngine.EpicStreamPriceAvailableCheck(OpenPosition.market.epic))
                                iGApiDbContext.SaveEpicTick(OpenPosition.market);
                        }
                    });
                }

                //  Remove closed positions (no longer existing).
                iGApiDbContext.OpenPositions.RemoveRange(
                    iGApiDbContext.OpenPositions
                        .Where(w => w.AccountId == currentAccountId).ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !currentApiOpenPositions.Any(b => b.position.dealId == a.DealId)));

                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
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