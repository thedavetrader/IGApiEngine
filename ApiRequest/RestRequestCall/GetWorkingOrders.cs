using IGApi.Common;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public void GetWorkingOrders()
        {
            try
            {
                string? currentAccountId = _apiEngine.LoginSessionInformation?.currentAccountId;
                _ = currentAccountId ?? throw new NullReferenceException(nameof(currentAccountId));

                var response = _apiEngine.IGRestApiClient.workingOrdersV2().UseManagedCall();

                if (response is not null)
                {
                    using IGApiDbContext iGApiDbContext = new();
                    _ = iGApiDbContext.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.WorkingOrders));
                    _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

                    var currentApiWorkingOrders = response.Response.workingOrders;

                    RemoveObsoleteEpicTicks(iGApiDbContext);

                    SyncToDbWorkingOrders(currentAccountId, iGApiDbContext, currentApiWorkingOrders);

                    SyncToEpicStreamList(currentApiWorkingOrders);

                    var parameters = currentApiWorkingOrders.Select(s => new { s.marketData.epic }).Distinct();

                    if (parameters.Any())
                    {
                        string? jsonParameters = null;

                        jsonParameters = JsonConvert.SerializeObject(
                            parameters,
                            Formatting.None);

                        ApiRequestItem.QueueItem(nameof(ApiRequestItem.GetEpicDetails), true, false, jsonParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetWorkingOrders));
                throw;
            }

            static void SyncToDbWorkingOrders(string? currentAccountId, IGApiDbContext iGApiDbContext, List<dto.endpoint.workingorders.get.v2.WorkingOrder> currentApiWorkingOrders)
            {
                if (currentApiWorkingOrders.Any())
                {
                    //  Upsert position response.
                    currentApiWorkingOrders.ForEach(workingOrder =>
                    {
                        if (workingOrder is not null)
                        {
                            // Upsert WorkingOrder
                            iGApiDbContext.SaveWorkingOrder(workingOrder.workingOrderData, currentAccountId);

                            // Save market data (only Epics where streamingpricesavailable.)
                            if (ApiEngine.EpicStreamPriceAvailableCheck(workingOrder.marketData.epic))
                                iGApiDbContext.SaveEpicTick(workingOrder.marketData);
                        }
                    });
                }

                //  Remove closed positions (no longer existing).
                iGApiDbContext.WorkingOrders.RemoveRange(
                    iGApiDbContext.WorkingOrders
                        .Where(w => w.AccountId == currentAccountId).ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !currentApiWorkingOrders.Any(b => b.workingOrderData.dealId == a.DealId)));

                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
            }

            void SyncToEpicStreamList(List<dto.endpoint.workingorders.get.v2.WorkingOrder> currentApiWorkingOrders)
            {
                if (currentApiWorkingOrders.Any())
                {
                    var epicStreamListItems = currentApiWorkingOrders
                        .Where(w => w.marketData.streamingPricesAvailable)
                        .Select(s => new EpicStreamListItem(s.marketData.epic, EpicStreamListItem.EpicStreamListItemSource.SourceWorkingOrders))
                        .Distinct()
                        .ToList();

                    _apiEngine.SyncEpicStreamListItems(epicStreamListItems, EpicStreamListItem.EpicStreamListItemSource.SourceWorkingOrders);
                }
            }
        }
    }
}