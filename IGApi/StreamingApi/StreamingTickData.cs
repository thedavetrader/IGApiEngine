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
    public class EpicStreamListItem
    {
        public readonly string Epic;

        public EpicStreamListItem(string epic)
        {
            Epic = epic;
        }
    }

    public sealed partial class ApiEngine
    {
        public ListExtension<EpicStreamListItem> EpicStreamList = new();

        // LS subscriptions...
        private L1PricesSubscription? _tickSubscription;
        private SubscribedTableKey? __tickSubscribedTableKey = new();

        /// <summary>
        /// Init everything for StreamingDataInit. Should only be called by constructor of IGApiEngine.
        /// </summary>
        private void StreamingTickDataInit()
        {
            EpicStreamList.ListChanged += EpicStreamListChanged;

            _tickSubscription = new(EpicStreamList);
            __tickSubscribedTableKey = null; //TODO: Important to initialise to null (?According to IGWebApi-sample!).
        }

        private void EpicStreamListChanged(object? sender, EventArgs e)
        {
            Task.Run(() => ReSubscribeToTickDetails());

            #region RestRequestQueue
            using IGApiDbContext iGApiDbContext = new();
            _ = iGApiDbContext.RestRequestQueue ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.RestRequestQueue));

            // TODO: formalize restRequestQueueItem string "GetEpicDetails". Make const class with possible values. Also make the check constraint dynamic based on this.
            string parameters = JsonConvert.SerializeObject(EpicStreamList, Formatting.None);

            RestRequestQueue restRequestQueueItem = new("GetEpicDetails", parameters, true, false);

            iGApiDbContext.SaveRestRequestQueue(restRequestQueueItem);

            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();
            #endregion
        }

        private void GetEpicDetails()
        {
            // TODO: GetEpicDetails
            // for each epic in epic stream list
            //  of which 
            //      epicDetails does not exists OR epicDetails data x(configmanager) days old
            // Insert into restrequestqueue.GetEpicDetails.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The list of epics to subscribe to has changed: An epic has been added or has been removed (with RemoveAll), in which case the CollectionChanged event is triggered.
        /// All current subscriptions should be cancelled and the epics in the curren up-to-date list should be registered.
        /// </summary>
        private void ReSubscribeToTickDetails()
        {
            try
            {
                lock (EpicStreamList)
                {
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", ""));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", "(Re-)subscribing epics to tick Lightning streamer."));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", ""));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", "Epic"));
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", new string('_', 40)));

                    foreach (var epic in EpicStreamList)
                        Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", epic.ToString()));

                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTickData]", "", "", ""));

                    UnsubscribeFromTickDetails();

                    if (EpicStreamList.Any())
                        SubscribeToTickDetails();
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(ReSubscribeToTickDetails));
                throw;
            }
        }

        private void SubscribeToTickDetails()
        {
            try
            {
                _ = LoginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation));

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint))
                {
                    var subscribeEpicList = EpicStreamList.Select(e => e.Epic).Distinct().ToList();

                    __tickSubscribedTableKey = _iGStreamApiClient.SubscribeToMarketDetails(subscribeEpicList, _tickSubscription);

                    Log.WriteLine("Successfully subscribed to positions");
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(SubscribeToTickDetails));
                throw;
            }
        }

        private void UnsubscribeFromTickDetails()
        {
            if (LoginSessionInformation is not null)
            {
                if (__tickSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(__tickSubscribedTableKey);
                    __tickSubscribedTableKey = null;

                    Log.WriteLine("Successfully unsubscribed from position subscription.");
                }
            }
        }

        /// <summary>
        /// This class inherits from HandyTableListenerAdapter and overrides its OnUpdate method that is called by the Lightstream api.
        /// </summary>
        public class L1PricesSubscription : HandyTableListenerAdapter
        {
            private readonly List<EpicStreamListItem> Epics;

            public L1PricesSubscription(List<EpicStreamListItem> epics)
            {
                Epics = epics;
            }

            public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
            {
                try
                {
                    using IGApiDbContext iGApiDbContext = new();
                    _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

                    var posUpdate = L1LsPriceUpdateData(itemPos, itemName, update);
                    var onUpdateEpic = itemName.Replace("L1:", "");

                    var position = Epics.Where(epic => epic.Epic == onUpdateEpic).FirstOrDefault();
                    {
                        Task.Run(async () => await iGApiDbContext.SaveEpicTick(posUpdate, onUpdateEpic).SaveChangesAsync()).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex, nameof(OnUpdate));
                    throw;
                }
            }
        }

        // TODO: SubscribeEpicsTickDetails, use EpicStreamListItem for validepics and invalid epics.
        public void SubscribeEpicsTickDetails(IEnumerable<string> validEpics, IEnumerable<string>? invalidEpics)
        {
            lock (EpicStreamList)
            {
                bool isChanged = false;

                //  Add new open epics
                List<string> newEpics = validEpics.Where(epic => !EpicStreamList.Where(item => item.Epic == epic).Any()).ToList();
                
                if (newEpics.Any())
                {
                    var EpicStreamListItemList = newEpics.Select(epic => new EpicStreamListItem(epic)).ToList();

                    EpicStreamList.AddRange(EpicStreamListItemList);

                    isChanged = true;
                }

                //  Remove closed epics.
                // TODO: Remove closed epics. It looks like this might not work.....
                if (EpicStreamList.RemoveAll(item => !validEpics.Contains(item.Epic)) > 0)
                    isChanged = true;

                //  Notify change once list is properly updated.
                if (isChanged)
                {
                    if (invalidEpics is not null && invalidEpics.Any())
                        foreach (var epic in invalidEpics)
                        {
                            LogInvalidEpic(epic);
                        }

                    EpicStreamList.NotifyChange();
                }
            }
        }

        public static void LogInvalidEpic(string epic)
        {
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", ""));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", "Streaming prices unavailable for:"));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", ""));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", "Epic"));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", new string('_', 40)));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", epic.ToString()));
            Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[GetOpenPositionDetails]", "", "", ""));
        }

        public void SubscribeEpicTickDetails(EpicStreamListItem epicStreamListItem)
        {
            lock (EpicStreamList)
            {
                //  Add new open Epic
                EpicStreamList.Add(epicStreamListItem);

                //  Notify change once list is properly updated.
                EpicStreamList.NotifyChange();
            }
        }

        public void UnsubscribeEpicTickDetails(EpicStreamListItem epicStreamListItem)
        {
            lock (EpicStreamList)
            {
                //  Remove closed Epic.
                EpicStreamList.Remove(epicStreamListItem);

                //  Notify change once list is properly updated.
                EpicStreamList.NotifyChange();
            }
        }
    }
}
