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

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetOpenPositions()
        {
            try
            {
                var response = _apiEngine.IGRestApiClient.getOTCOpenPositionsV2().UseManagedCall();

                using ApiDbContext apiDbContext = new();

                var currentApiOpenPositions = response.Response.positions;

                RemoveObsoleteEpicTicks(apiDbContext);

                SyncToDbOpenPositions(_currentAccountId, apiDbContext, currentApiOpenPositions);
                
                SyncToEpicStreamList(currentApiOpenPositions);
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

            void SyncToDbOpenPositions(string currentAccountId, ApiDbContext apiDbContext, List<dto.endpoint.positions.get.otc.v2.OpenPosition> currentApiOpenPositions)
            {
                if (currentApiOpenPositions.Any())
                {
                    //  Upsert position response.
                    currentApiOpenPositions.ForEach(openPosition =>
                    {
                        if (openPosition is not null)
                        {
                            // DO NOT Save market data (only Epics where streamingpricesavailable.)
                            //      It appears this data is often out of date or incomplete (no offer and bid prices)
                            //new EpicTick(openPosition.market).SaveEpicTick(new ApiDbContext().ConnectionString); // !!! IMPORTANT NOTICE: this snapshot data is the marketdata @time of execution NOT @time of creation position
                            apiDbContext.SaveOpenPosition(openPosition.position, currentAccountId, openPosition.market.epic);
                        }
                    });
                }

                //  Remove closed positions (no longer existing).
                apiDbContext.OpenPositions.RemoveRange(
                    apiDbContext.OpenPositions
                        .Where(w => w.AccountId == currentAccountId).ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !currentApiOpenPositions.Any(b => b.position.dealId == a.DealId)));

                Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
            }

            void SyncToEpicStreamList(List<dto.endpoint.positions.get.otc.v2.OpenPosition> currentApiOpenPositions)
            {
                if (currentApiOpenPositions.Any())
                {
                    var epicStreamListItems = currentApiOpenPositions.ToList()
                        .Where(w => w.market.streamingPricesAvailable)
                        .GroupBy(g => g.market.epic)
                        .Select(s => new EpicStreamListItem(s.First().market.epic, EpicStreamListItem.EpicStreamListItemSource.OpenPositions))
                        .ToList()
                        ;

                    _apiEngine.SyncEpicStreamListItems(epicStreamListItems, EpicStreamListItem.EpicStreamListItemSource.OpenPositions);                   
                }
            }
        }
    }
}