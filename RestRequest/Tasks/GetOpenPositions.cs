using System.Globalization;
using IGApi.Model;
using IGApi.Common;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        private void GetOpenPositionDetails()
        {
            try
            {
                string? currentAccountId = _iGApiEngine.LoginSessionInformation?.currentAccountId;
                _ = currentAccountId ?? throw new NullReferenceException(nameof(currentAccountId));

                using IGApiDbContext iGApiDbContext = new();
                _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));

                #region RestRequestCall
                var response = Task.Run(async () => await _iGApiEngine.IGRestApiClient.getOTCOpenPositionsV2()).Result;
                _ = response ?? throw new RestCallNullReferenceException(nameof(response));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new RestCallHttpRequestException(nameof(GetOpenPositionDetails), response.StatusCode);

                var currentPositions = response.Response.positions;
                #endregion RestRequestCall

                #region SyncWithDb
                //  Upsert position response.
                currentPositions.ForEach(OpenPosition =>
                {
                    if (OpenPosition is not null)
                    {
                        // Upsert OpenPosition
                        iGApiDbContext.SaveOpenPosition(OpenPosition.position, currentAccountId, OpenPosition.market.epic);

                        // Save market data
                        iGApiDbContext.SaveEpicTick(OpenPosition.market);
                    }
                });

                //  Remove closed positions (no longer existing).
                var currentDeals = currentPositions
                    .Select(s => s.position.dealId);

                var closedPositions = iGApiDbContext.OpenPositions
                    .Where(w =>
                        !(
                            w.AccountId == currentAccountId &&
                            currentDeals.Any(DealId => DealId == w.DealId)
                        ));

                iGApiDbContext.OpenPositions.RemoveRange(closedPositions);
                #endregion

                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

                #region EpicStreamList
                var validEpics = currentPositions
                    .Where(w => !w.market.streamingPricesAvailable)
                    .Select(s => s.market.epic);

                var invalidEpics = currentPositions
                    .Where(w => w.market.streamingPricesAvailable)  // Only for epics who are available for streaming.
                    .Select(s => s.market.epic);

                _iGApiEngine.SubscribeEpicsTickDetails(invalidEpics, validEpics);
                #endregion
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositionDetails));
                throw;
            }
        }
        
    }
}