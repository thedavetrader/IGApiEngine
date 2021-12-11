using System.Globalization;
using System.Text;
using dto.endpoint.positions.get.otc.v1;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;
using Newtonsoft.Json;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        private SubscribedTableKey? _tradeSubscribedTableKey = new();
        private TradeSubscription? _tradeSubscription;

        /// <summary>
        /// Init everything for StreamingDataInit. Should only be called by constructor of IGApiEngine.
        /// </summary>
        private void StreamingTradeDataInit()
        {
            _tradeSubscription = new(this);
        }

        /// <summary>
        /// The list of Trades to subscribe to has changed: An Trade has been added or has been removed (with RemoveAll), in which case the CollectionChanged event is triggered.
        /// All current subscriptions should be cancelled and the Trades in the curren up-to-date list should be registered.
        /// </summary>
        private void SubscribeToTradeDetails()
        {
            try
            {
                _ = LoginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation));

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint))
                {
                    _tradeSubscribedTableKey = _iGStreamApiClient.SubscribeToTradeSubscription(LoginSessionInformation.currentAccountId, _tradeSubscription);
                    Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatTwoColumns, "[StreamingTickData]", "Subscribed to confirms(CONFIRMS), working order updates(WOU) and open position updates(OPU)"));
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(SubscribeToTradeDetails));
                throw;
            }
        }

        private void UnsubscribeFromTradeDetails()
        {
            if (LoginSessionInformation is not null)
            {
                if (_tradeSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(_tradeSubscribedTableKey);
                    _tradeSubscribedTableKey = null;

                    Log.WriteLine("Successfully unsubscribed from trade subscription.");
                }
            }
        }

        /// <summary>
        /// This class inherits from HandyTableListenerAdapter and overrides its OnUpdate method that is called by the Lightstream api.
        /// </summary>
        public class TradeSubscription : HandyTableListenerAdapter
        {
            private readonly ApiEngine _iGApiEngine;
            enum UpdateType { OpenPosition, WorkingOrder, Confirms };

            public TradeSubscription(ApiEngine iGApiEngine)
            {
                _iGApiEngine = iGApiEngine;
            }

            public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Trade Subscription Update");

                try
                {
                    var opu = update.GetNewValue("OPU");
                    //BUG @ IG. Working Order updates are also labeled as OPU.
                    var wou = update.GetNewValue("OPU");
                    var confirms = update.GetNewValue("CONFIRMS");
                    UpdateType updateType = UpdateType.Confirms;

                    //TODO: _PRIO SO identify WOU if contains field timeInForce
                    updateType =
                        String.IsNullOrEmpty(confirms) &&
                        !string.IsNullOrEmpty(opu)
                        && opu.Contains("timeInForce") ? UpdateType.WorkingOrder : UpdateType.OpenPosition;

                    string? inputData = null;

                    if (!(String.IsNullOrEmpty(opu)) && updateType == UpdateType.OpenPosition)
                    {
                        inputData = opu;
                        Log.WriteLine("OPU");
                    }
                    else if (!(String.IsNullOrEmpty(wou)) && updateType == UpdateType.WorkingOrder)
                    {
                        inputData = wou;
                        Log.WriteLine("WOU");
                    }
                    else if (!(String.IsNullOrEmpty(confirms)) && updateType == UpdateType.Confirms)
                    {
                        inputData = confirms;
                        Log.WriteLine("CONFIRMS");
                    }

                    if (!(String.IsNullOrEmpty(inputData)))
                        SyncToDb(itemName, updateType, inputData);
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex, nameof(OnUpdate));
                    throw;
                }

                void SyncToDb(string itemName, UpdateType updateType, string inputData)
                {
                    var accountId = itemName.Replace("TRADE:", "");

                    var openPositionData = JsonConvert.DeserializeObject<LsTradeSubscriptionData>(inputData);
                    var workingOrderData = JsonConvert.DeserializeObject<dto.endpoint.workingorders.get.v2.WorkingOrderData>(inputData);
                    IGApiDbContext iGApiDbContext = new();

                    if (updateType == UpdateType.OpenPosition)
                    {
                        var status = openPositionData.status;
                        var epic = openPositionData.epic;
                        var epicStreamListItemSource = EpicStreamListItem.EpicStreamListItemSource.SourceOpenPositions;

                        if (status is not null && epic is not null)
                        {
                            SyncToDbOpenPositions(openPositionData, accountId, iGApiDbContext);

                            RegisterStreamEpic((StreamingStatusEnum)status, epic, epicStreamListItemSource);
                        }
                    }
                    else if (updateType == UpdateType.WorkingOrder)
                    {
                        var status = openPositionData.status;   // Working order does not contain status. And because of the bug, wou are sent as opu, so get the status from opu.
                        var epic = workingOrderData.epic;
                        var epicStreamListItemSource = EpicStreamListItem.EpicStreamListItemSource.SourceWorkingOrders;

                        if (status is not null && epic is not null)
                        {
                            SyncToDbWorkingOrders((StreamingStatusEnum)status, workingOrderData, accountId, iGApiDbContext);

                            RegisterStreamEpic((StreamingStatusEnum)status, epic, epicStreamListItemSource);
                        }
                    }
                    else if (updateType == UpdateType.Confirms)
                        throw new NotImplementedException("Streamupdates of the type \"Confirms\" are not supported.");

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();

                }

                void RegisterStreamEpic(StreamingStatusEnum status, string? epic, EpicStreamListItem.EpicStreamListItemSource epicStreamListItemSource)
                {
                    var epicStreamListItem =
                        _iGApiEngine.EpicStreamList.Find(f => f.Epic == epic) ??
                        new EpicStreamListItem(epic, (EpicStreamListItem.EpicStreamListItemSource)epicStreamListItemSource);

                    if (status == StreamingStatusEnum.OPEN)
                        _iGApiEngine.AddEpicStreamListItem(epicStreamListItem);
                    else if (status == StreamingStatusEnum.CLOSED || status == StreamingStatusEnum.DELETED)
                        _iGApiEngine.RemoveEpicStreamListItem(epicStreamListItem);
                }

                static void SyncToDbOpenPositions(LsTradeSubscriptionData openPositionData, string accountId, IGApiDbContext iGApiDbContext)
                {
                    _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));

                    if (
                        openPositionData.status == StreamingStatusEnum.OPEN ||
                        openPositionData.status == StreamingStatusEnum.UPDATED ||
                        openPositionData.status == StreamingStatusEnum.AMENDED)
                    {
                        iGApiDbContext.SaveOpenPosition(openPositionData, accountId);
                    }
                    else if (
                        openPositionData.status == StreamingStatusEnum.CLOSED ||
                        openPositionData.status == StreamingStatusEnum.DELETED)
                    {
                        var openPosition = Task.Run(async () => await iGApiDbContext.OpenPositions.FindAsync(accountId, openPositionData.dealId)).Result;

                        if (openPosition != null)
                            iGApiDbContext.Remove(openPosition);

                        //TODO: Notify GetTradeActivity
                    }
                }

                static void SyncToDbWorkingOrders(
                    StreamingStatusEnum status,
                    dto.endpoint.workingorders.get.v2.WorkingOrderData WorkingOrderData,
                    string accountId,
                    IGApiDbContext iGApiDbContext)
                {
                    _ = iGApiDbContext.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.WorkingOrders));

                    if (
                        status == StreamingStatusEnum.OPEN ||
                        status == StreamingStatusEnum.UPDATED ||
                        status == StreamingStatusEnum.AMENDED)
                    {
                        iGApiDbContext.SaveWorkingOrder(WorkingOrderData, accountId);
                    }
                    else if (
                        status == StreamingStatusEnum.CLOSED ||
                        status == StreamingStatusEnum.DELETED)
                    {
                        var WorkingOrder = Task.Run(async () => await iGApiDbContext.WorkingOrders.FindAsync(accountId, WorkingOrderData.dealId)).Result;

                        if (WorkingOrder != null)
                            iGApiDbContext.Remove(WorkingOrder);

                        //TODO: Notify GetTradeActivity
                    }
                }
            }
        }
    }
}
