using IGApi.Common;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetWorkingOrdersCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetWorkingOrders()
        {
            try
            {
                var response = _apiEngine.IGRestApiClient.workingOrdersV2().UseManagedCall();

                using ApiDbContext apiDbContext = new();

                var currentApiWorkingOrders = response.Response.workingOrders;

                RemoveObsoleteEpicTicks(apiDbContext);

                SyncToDbWorkingOrders(_currentAccountId, apiDbContext, currentApiWorkingOrders);

                SyncToEpicStreamList(currentApiWorkingOrders);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(GetWorkingOrdersCompleted);
            }

            void SyncToDbWorkingOrders(string currentAccountId, ApiDbContext apiDbContext, List<dto.endpoint.workingorders.get.v2.WorkingOrder> currentApiWorkingOrders)
            {
                if (currentApiWorkingOrders.Any())
                {
                    //  Upsert position response.
                    currentApiWorkingOrders.ForEach(workingOrder =>
                    {
                        if (workingOrder is not null)
                        {
                            // Upsert WorkingOrder
                            apiDbContext.SaveWorkingOrder(workingOrder.workingOrderData, currentAccountId);

                            // DO NOT Save market data (only Epics where streamingpricesavailable.)
                            //      It appears this data is often out of date or incomplete (no offer and bid prices)
                            //if (ApiEngine.EpicStreamPriceAvailableCheck(workingOrder.marketData.epic))
                            //    new EpicTick(workingOrder.marketData).SaveEpicTick(new ApiDbContext().ConnectionString);
                        }
                    });
                }

                //  Remove closed positions (no longer existing).
                apiDbContext.WorkingOrders.RemoveRange(
                    apiDbContext.WorkingOrders
                        .Where(w => w.AccountId == currentAccountId).ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !currentApiWorkingOrders.Any(b => b.workingOrderData.dealId == a.DealId)));

                Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
            }

            void SyncToEpicStreamList(List<dto.endpoint.workingorders.get.v2.WorkingOrder> currentApiWorkingOrders)
            {
                if (currentApiWorkingOrders.Any())
                {
                    var epicStreamListItems = currentApiWorkingOrders
                        .Where(w => w.marketData.streamingPricesAvailable)
                        .Select(s => new EpicStreamListItem(s.marketData.epic, EpicStreamListItem.EpicStreamListItemSource.WorkingOrders))
                        .Distinct()
                        .ToList();

                    _apiEngine.SyncEpicStreamListItems(epicStreamListItems, EpicStreamListItem.EpicStreamListItemSource.WorkingOrders);
                }
            }
        }
    }
}