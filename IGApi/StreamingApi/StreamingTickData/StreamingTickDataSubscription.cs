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
        // LS subscriptions...
        private L1PricesSubscription? _tickSubscription;
        private SubscribedTableKey? __tickSubscribedTableKey = new();

        /// <summary>
        /// Init everything for StreamingDataInit. Should only be called by constructor of IGApiEngine.
        /// </summary>
        private void StreamingTickDataInit()
        {
            EpicStreamList.ListChanged += EpicStreamListChanged;

            _tickSubscription = new(this);
            __tickSubscribedTableKey = null; //TODO: Important to initialise to null (?According to IGWebApi-sample!).
        }

        private void EpicStreamListChanged(object? sender, EventArgs e)
        {
            Task.Run(() => ReSubscribeToEpicTick());
        }

        /// <summary>
        /// The list of epics to subscribe to has changed: An epic has been added or has been removed (with RemoveAll), in which case the CollectionChanged event is triggered.
        /// All current subscriptions should be cancelled and the epics in the curren up-to-date list should be registered.
        /// </summary>
        private void ReSubscribeToEpicTick()
        {
            try
            {
                lock (EpicStreamList)
                {
                    UnsubscribeFromEpicTick();

                    if (EpicStreamList.Any())
                    {
                        SubscribeToEpicTick();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(ReSubscribeToEpicTick));
                throw;
            }
        }

        private void SubscribeToEpicTick()
        {
            try
            {
                _ = LoginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation));

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint))
                {
                    var subscribeEpicList = EpicStreamList.Select(e => e.Epic).Distinct().ToList();

                    __tickSubscribedTableKey = _iGStreamApiClient.SubscribeToMarketDetails(subscribeEpicList, _tickSubscription);
                    
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
                Log.WriteException(ex, nameof(SubscribeToEpicTick));
                throw;
            }
        }

        private void UnsubscribeFromEpicTick()
        {
            if (LoginSessionInformation is not null)
            {
                if (__tickSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(__tickSubscribedTableKey);
                    __tickSubscribedTableKey = null;

                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatTwoColumns, "[StreamingTickData]", "Unsubscribed all epics from tick Lightning streamer."));
                }
            }
        }

        /// <summary>
        /// This class inherits from HandyTableListenerAdapter and overrides its OnUpdate method that is called by the Lightstream api.
        /// </summary>
        public class L1PricesSubscription : HandyTableListenerAdapter
        {
            private readonly List<RestRequestParameterEpic> EpicStreamList;
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
                    var restQueueParameterEpic = new RestRequestParameterEpic(onUpdateEpic);

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
                        Log.WriteLine("Invalid subscription found, resubscribing.");
                        EpicStreamList.Remove(restQueueParameterEpic);
                        _apiEngine.ReSubscribeToEpicTick();
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