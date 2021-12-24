﻿using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetWatchListEpicsCompleted;
        //TODO: Make functionality that can create auto watchlists based on signals (let's begin with trending i.c.w. clientsentiment)
        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetWatchListEpics()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                using ApiDbContext apiDbContext = new();

                _ = apiDbContext.Watchlists ?? throw new DBContextNullReferenceException(nameof(apiDbContext.Watchlists));
                _ = apiDbContext.WatchlistEpicDetails ?? throw new DBContextNullReferenceException(nameof(apiDbContext.WatchlistEpicDetails));
                _ = apiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetails));

                var response = _apiEngine.IGRestApiClient.instrumentsForWatchlist(request).UseManagedCall();

                if (response is not null)
                {
                    var markets = response.Response.markets;
                    var newEpics = markets.Where(w => !apiDbContext.EpicDetails.Any(a => a.Epic == w.epic));

                    // First retreive epic detail if not yet exists on db, preventing FK errors inserting WatchlistEpicDetails.
                    if (newEpics is not null && newEpics.Any())
                    {
                        var parameters = newEpics.Select(s => new { s.epic }).Distinct();

                        if (parameters.Any())
                        {
                            string? jsonParameters = null;

                            jsonParameters = JsonConvert.SerializeObject(
                                parameters,
                                Formatting.None);

                            var getEpicDetailsGuid = Guid.NewGuid();

                            // Using eventhandler to queue this when done. Otherwise it would fire almost immediately, while the epic details are still beeing retreived.
                            void GetEpicDetailsCompleted(object? sender, EventArgs e)
                            {
                                if (sender is not null)
                                {
                                    var senderEngineItem = (RequestQueueEngineItem)sender;

                                    if (senderEngineItem.ApiRequestQueueItem.Guid == getEpicDetailsGuid)
                                    {
                                        RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetWatchListEpics), true, false, Guid.NewGuid(), null, request);
                                        RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;
                                    }
                                }
                            }

                            RequestQueueEngineItem.GetEpicDetailsCompleted -= GetEpicDetailsCompleted;    // Prevent event is subscribed more then once.
                            RequestQueueEngineItem.GetEpicDetailsCompleted += GetEpicDetailsCompleted;

                            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetEpicDetails), true, false, getEpicDetailsGuid, null, jsonParameters);
                        }
                    }
                    else
                    {
                        //  Upsert
                        markets.ForEach(market =>
                        {
                            if (market is not null)
                                apiDbContext.SaveWatchlistEpicDetail(_currentAccountId, request, market);

                            //TODO Sync with streaming list.
                        });

                        // Remove obsolete
                        var watchlist = apiDbContext.Watchlists.Find(_currentAccountId, request);

                        if (watchlist is not null)
                        {
                            var watchlistEpicDetails = watchlist.WatchlistEpicDetails;

                            if (watchlistEpicDetails is not null)
                            {
                                apiDbContext.RemoveRange(watchlistEpicDetails
                                    .Where(w => w.AccountId == _currentAccountId).ToList()
                                    .Where(y => !response.Response.markets.Any(m => m.epic == y.Epic))
                                    );
                            }
                        }

                        Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
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
                QueueItemComplete(GetWatchListEpicsCompleted);
            }
        }
    }
}