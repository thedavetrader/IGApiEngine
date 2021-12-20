﻿using System.Globalization;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using IGApi.Model;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private SubscribedTableKey? _tickSubscribedTableKey = null;
        private L1PricesSubscription? _tickSubscription;

        /// <summary>
        /// Init everything for StreamingDataInit. Should only be called by constructor of IGApiEngine.
        /// </summary>
        private void StreamingTickDataInit()
        {
            EpicStreamList.ListChanged += EpicStreamListChanged;

            _tickSubscription = new(this);
        }

        private void EpicStreamListChanged(object? sender, EventArgs e)
        {
            Task.Run(() => ReSubscribeToAllEpicTick());
        }

        /// <summary>
        /// The list of epics to subscribe to has changed: An epic has been added or has been removed (with RemoveAll), in which case the CollectionChanged event is triggered.
        /// All current subscriptions should be cancelled and the epics in the curren up-to-date list should be registered.
        /// </summary>
        private void ReSubscribeToAllEpicTick()
        {
            try
            {
                lock (EpicStreamList)
                {
                    UnsubscribeFromAllEpicTick();

                    if (EpicStreamList.Any())
                    {
                        SubscribeToAllEpicTick();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }

        private void SubscribeToAllEpicTick()
        {
            try
            {
                if (!IsLoggedIn) throw new IGApiConncectionError();

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint) && EpicStreamList.Any())
                {
                    var subscribeEpicList = EpicStreamList.Select(e => e.Epic).Distinct().ToList();

                    _tickSubscribedTableKey = _iGStreamApiClient.SubscribeToMarketDetails(subscribeEpicList, _tickSubscription);

                    WriteLog();
                    WriteLog(Messages("(Re-)subscribed epics to tick Lightning streamer."));
                    WriteLog();
                    WriteLog(Messages("Epic"));
                    WriteLog(Messages(new string('_', 40)));

                    foreach (var epic in subscribeEpicList)
                        WriteLog(Messages(epic));

                    WriteLog();
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }

        private void UnsubscribeFromAllEpicTick()
        {
            if (IsLoggedIn)
            {
                if (_tickSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(_tickSubscribedTableKey);
                    _tickSubscribedTableKey = null;

                    WriteLog(Messages("Unsubscribed all epics from tick Lightning streamer."));
                }
            }
        }

        /// <summary>
        /// This class inherits from HandyTableListenerAdapter and overrides its OnUpdate method that is called by the Lightstream api.
        /// </summary>
        public class L1PricesSubscription : HandyTableListenerAdapter
        {
            private readonly ObservableList<EpicStreamListItem> EpicStreamList;
            private readonly ApiEngine _apiEngine;

            public L1PricesSubscription(ApiEngine apiEngine)
            {
                _apiEngine = apiEngine;
                EpicStreamList = apiEngine.EpicStreamList;
            }

            public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
            {
                try
                {
                    var tickUpdate = L1LsPriceUpdateData(itemPos, itemName, update);
                    var onUpdateEpic = itemName.Replace("L1:", "");

                    using IGApiDbContext iGApiDbContext = new();
                    _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

                    iGApiDbContext.SaveEpicTick(tickUpdate, onUpdateEpic);

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();

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