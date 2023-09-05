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

            _tickSubscription = new();
        }

        /// <summary>
        /// The list of epics to subscribe to has changed: An epic has been added or has been removed (with RemoveAll), in which case the CollectionChanged event is triggered.
        /// All current subscriptions should be cancelled and the epics in the curren up-to-date list should be registered.
        /// </summary>
        private void ReSubscribeToAllEpicTick()
        {
            lock (EpicStreamList)
            {
                RunRetry(() => UnsubscribeFromAllEpicTick());

                if (EpicStreamList.Any())
                {
                    RunRetry(() => SubscribeToAllEpicTick());
                }
            }
        }

        private static void RunRetry(Action func)
        {
            bool retry = true;
            int timeout = 20;

            while (retry && timeout > 0)
            {
                try
                {
                    func();
                    retry = false;
                }
                catch (Exception ex)
                {
                    if (ex is SubscrException)
                    {
                        WriteException(ex);
                        timeout--;
                    }
                    else
                        throw;
                }
            }
        }

        private void SubscribeToAllEpicTick()
        {
            try
            {
                if (!IsLoggedIn && !_cancellationToken.IsCancellationRequested) throw new IGApiConncectionError();

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint) && EpicStreamList.Any())
                {
                    var subscribeEpicList = EpicStreamList.Select(s => s.Epic).Distinct().ToList();

                    _tickSubscribedTableKey = _iGStreamApiClient.SubscribeToMarketDetails(subscribeEpicList, _tickSubscription);

                    WriteLog();
                    WriteLog(Columns("(Re-)subscribed epics to tick Lightning streamer."));
                    WriteLog();
                    WriteLog(Columns("Epic"));
                    WriteLog(Columns(new string('_', 40)));

                    foreach (var epic in subscribeEpicList)
                        WriteLog(Columns(epic));

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
            using ApiDbContext apiDbContext = new();

            //apiDbContext.EpicTicks.RemoveRange(apiDbContext.EpicTicks.ToList().Where(w => !EpicStreamList.Any(a => a.Epic == w.Epic)));

            Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();

            if (IsLoggedIn)
            {
                if (_tickSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(_tickSubscribedTableKey);
                    _tickSubscribedTableKey = null;

                    WriteLog(Columns("Unsubscribed all epics from tick Lightning streamer."));
                }
            }
        }

        /// <summary>
        /// This class inherits from HandyTableListenerAdapter and overrides its OnUpdate method that is called by the Lightstream api.
        /// </summary>
        public class L1PricesSubscription : HandyTableListenerAdapter
        {
            public L1PricesSubscription() { }

            public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
            {
                try
                {
                    var tickUpdate = L1LsPriceUpdateData(itemPos, itemName, update);
                    var onUpdateEpic = itemName.Replace("L1:", "");

                    var epicTick = new EpicTick(tickUpdate, onUpdateEpic);

                    if (epicTick.Offer is not null && epicTick.Bid is not null)
                        epicTick.SaveEpicTick(new ApiDbContext().ConnectionString);
                    else
                        Log.WriteWarning(Columns($"[WARNING] No offer and/or bid data for epic {epicTick.Epic}. If this persists, consider restarting the api."));
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