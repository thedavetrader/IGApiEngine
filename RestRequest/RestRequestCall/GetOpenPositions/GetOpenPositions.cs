using System.Globalization;
using IGApi.Model;
using IGApi.Common;

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

                using IGApiDbContext iGApiDbContext = new();
                _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));
                _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

                #region RestRequestCall
                var response = GetOpenPositionsResponse();
                #endregion RestRequestCall

                if (response is not null)
                {
                    #region SyncWithDb
                    //  Remoe EpicTicks of which streaming prices are not available. These are leftovers from when a position was opened, despite that 
                    //  at that time it could not be determined wether streamprices were available for that the particular epic of that position.
                    var currentDbEpicTicks = iGApiDbContext.EpicTicks.ToList();

                    currentDbEpicTicks.ForEach(currentEpicTick =>
                    {
                        if (!ApiEngine.EpicStreamPriceAvailableCheck(currentEpicTick.Epic))
                            iGApiDbContext.EpicTicks.Remove(currentEpicTick);
                    });

                    var currentApiOpenPositions = response.Response.positions;

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

                    //  Remove closed positions (no longer existing).
                    var currentApiDeals = currentApiOpenPositions
                        .Select(s => s.position.dealId);

                    var closedDbPositions = iGApiDbContext.OpenPositions
                        .Where(w =>
                            !(
                                w.AccountId == currentAccountId &&
                                currentApiDeals.Any(DealId => DealId == w.DealId)
                            ));

                    iGApiDbContext.OpenPositions.RemoveRange(closedDbPositions);
                    #endregion

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

                    #region EpicStreamList
                    var epicStreamListItems = currentApiOpenPositions
                        .Where(w => w.market.streamingPricesAvailable)
                        .Select(s => new EpicStreamListItem(s.market.epic, usedByOpenPositions: true))
                        .Distinct()
                        .ToList();

                    _apiEngine.SyncEpicStreamListItems(epicStreamListItems);
                    #endregion

                    #region GetEpicDetails
                    RestQueueQueueItem.RestQueueGetEpicDetails(currentApiOpenPositions.Select(s => new EpicStreamListItem(s.market.epic)).ToList());
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }
        }

    }
}