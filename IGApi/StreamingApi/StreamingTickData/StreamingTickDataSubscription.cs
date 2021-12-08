using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;
using Newtonsoft.Json;

namespace IGApi
{
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

         //   _tickSubscribedTableKey = null;
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
                Log.WriteException(ex, nameof(ReSubscribeToAllEpicTick));
                throw;
            }
        }

        private void SubscribeToAllEpicTick()
        {
            try
            {
                _ = LoginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation));

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint))
                {
                    var subscribeEpicList = EpicStreamList.Select(e => e.Epic).Distinct().ToList();

                    _tickSubscribedTableKey = _iGStreamApiClient.SubscribeToMarketDetails(subscribeEpicList, _tickSubscription);
                    
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[StreamingTickData]", "", "", ""));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatTwoColumns, "[StreamingTickData]", "(Re-)subscribed epics to tick Lightning streamer."));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[StreamingTickData]", "", "", ""));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[StreamingTickData]", "", "", "Epic"));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[StreamingTickData]", "", "", new string('_', 40)));

                    foreach (var epic in EpicStreamList)
                        Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[StreamingTickData]", "", "", epic.Epic));

                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatFourColumns, "[StreamingTickData]", "", "", ""));
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(SubscribeToAllEpicTick));
                throw;
            }
        }

        private void UnsubscribeFromAllEpicTick()
        {
            if (LoginSessionInformation is not null)
            {
                if (_tickSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(_tickSubscribedTableKey);
                    _tickSubscribedTableKey = null;

                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatTwoColumns, "[StreamingTickData]", "Unsubscribed all epics from tick Lightning streamer."));
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

                    if (EpicStreamPriceAvailableCheck(onUpdateEpic))
                    {
                        using IGApiDbContext iGApiDbContext = new();
                        _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

                        var existingEpicTick = EpicStreamList.Where(epic => epic.Epic == onUpdateEpic).FirstOrDefault();
                        {
                            iGApiDbContext.SaveEpicTick(tickUpdate, onUpdateEpic);
                            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
                        }
                    }
                    else
                    {
                        Log.WriteLine($"No streaming prices available for epic {onUpdateEpic}.");
                        
                        var epicStreamListItem = EpicStreamList.Find(f => f.Epic == onUpdateEpic);

                        if (epicStreamListItem is not null)
                        {
                            EpicStreamList.Remove(epicStreamListItem);

                            //  For this epic to be unsubscribed, all epics have to unsubscribe an re-subscribe.
                            //  There is no way to (un-)subscribe a individual epic.
                            _apiEngine.ReSubscribeToAllEpicTick();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex, nameof(OnUpdate));
                    throw;
                }
            }
        }    
    }
}