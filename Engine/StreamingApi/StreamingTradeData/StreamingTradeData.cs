﻿using System.Globalization;
using System.Text;
using dto.endpoint.positions.get.otc.v1;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;
using Newtonsoft.Json;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using System.Diagnostics;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private SubscribedTableKey? _tradeSubscribedTableKey = null;
        private TradeSubscription? _tradeSubscription;

        /// <summary>
        /// Init everything for StreamingDataInit. Should only be called by constructor of IGApiEngine.
        /// </summary>
        private void StreamingTradeDetailsInit()
        {
            _tradeSubscription = new(this, _cancellationToken);
        }

        private void ReSubscribeTradeDetails()
        {
            try
            {
                UnsubscribeFromTradeDetails();

                SubscribeToTradeDetails();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// The list of Trades to subscribe to has changed: An Trade has been added or has been removed (with RemoveAll), in which case the CollectionChanged event is triggered.
        /// All current subscriptions should be cancelled and the Trades in the curren up-to-date list should be registered.
        /// </summary>
        private void SubscribeToTradeDetails()
        {
            try
            {
                if (!IsLoggedIn && !_cancellationToken.IsCancellationRequested) throw new IGApiConncectionError();

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint))
                {
                    _tradeSubscribedTableKey = _iGStreamApiClient.SubscribeToTradeSubscription(LoginSessionInformation.currentAccountId, _tradeSubscription);

                    if (_tradeSubscribedTableKey is not null)
                        WriteLog(Columns("Subscribed to confirms(CONFIRMS), working order updates(WOU) and open position updates(OPU)"));
                    else
                        WriteLog(Columns("Failed to subscribe to confirms(CONFIRMS), working order updates(WOU) and open position updates(OPU)"));
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }

        private void UnsubscribeFromTradeDetails()
        {
            if (IsLoggedIn)
            {
                if (_tradeSubscribedTableKey is not null)
                {
                    _iGStreamApiClient.UnsubscribeTableKey(_tradeSubscribedTableKey);
                    _tradeSubscribedTableKey = null;

                    WriteLog(Columns("Successfully unsubscribed from trade subscription."));
                }
            }
        }

        /// <summary>
        /// This class inherits from HandyTableListenerAdapter and overrides its OnUpdate method that is called by the Lightstream api.
        /// </summary>
        public class TradeSubscription : HandyTableListenerAdapter
        {
            private readonly ApiEngine _iGApiEngine;

            private readonly CancellationToken _cancellationToken;
            enum UpdateType { OpenPosition, WorkingOrder, Confirms, Unknown };

            public TradeSubscription(ApiEngine iGApiEngine, CancellationToken cancellationToken)
            {
                _iGApiEngine = iGApiEngine;
                _cancellationToken = cancellationToken;
            }

            public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Trade Subscription Update");

                try
                {
                    UpdateType updateType = UpdateType.Unknown;

                    string? updateData = null;

                    updateData = update.GetNewValue("CONFIRMS");
                    if (!String.IsNullOrEmpty(updateData))
                    {
                        updateType = UpdateType.Confirms;
                    }
                    else
                    {
                        //BUG @ IG. Working Order updates are also labeled as OPU.
                        updateData = update.GetNewValue("OPU");

                        if (!String.IsNullOrEmpty(updateData))
                        {
                            if (updateData.Contains("goodTillDate"))
                            {
                                updateType = UpdateType.WorkingOrder;
                            }
                            else
                            {
                                updateType = UpdateType.OpenPosition;
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(updateData) && updateType != UpdateType.Unknown)
                        SyncToDb(itemName, updateType, updateData);
                }
                catch (Exception ex)
                {
                    WriteException(ex);

                    throw;
                }

                void SyncToDb(string itemName, UpdateType updateType, string updateData)
                {
                    var accountId = itemName.Replace("TRADE:", "");

                    if (updateType == UpdateType.OpenPosition || updateType == UpdateType.WorkingOrder)
                    {
                        //  Always deserialize LsTradeSubscriptionData. It holds the update status that is used by OpenPosition as well as WorkingOrder.
                        var openPositionData = JsonConvert.DeserializeObject<LsTradeSubscriptionData>(updateData);

                        if (updateType == UpdateType.OpenPosition)
                        {
                            var status = openPositionData.status;
                            var epic = openPositionData.epic;
                            var epicStreamListItemSource = EpicStreamListItem.EpicStreamListItemSource.OpenPositions;

                            if (status is not null && epic is not null)
                            {
                                SyncToDbOpenPositions(openPositionData, accountId);

                                RegisterStreamEpic((StreamingStatusEnum)status, epic, epicStreamListItemSource);

                                LogUpdateInfo(updateType, epic, openPositionData.size, openPositionData.direction.ToString(), openPositionData.dealStatus.ToString());
                            }
                        }
                        else if (updateType == UpdateType.WorkingOrder)
                        {
                            var workingOrderData = JsonConvert.DeserializeObject<dto.endpoint.workingorders.get.v2.WorkingOrderData>(updateData);
                            var status = openPositionData.status;   // Working order does not contain status. And because of the bug, wou are sent as opu, so get the status from opu.
                            var epic = workingOrderData.epic;
                            var epicStreamListItemSource = EpicStreamListItem.EpicStreamListItemSource.WorkingOrders;

                            if (status is not null && epic is not null)
                            {
                                SyncToDbWorkingOrders((StreamingStatusEnum)status, workingOrderData, accountId);

                                RegisterStreamEpic((StreamingStatusEnum)status, epic, epicStreamListItemSource);

                                LogUpdateInfo(updateType, epic, (workingOrderData.orderSize ?? 0).ToString(), workingOrderData.direction, openPositionData.dealStatus.ToString());
                            }
                        }
                    }
                    else if (updateType == UpdateType.Confirms)
                    {
                        var confirmData = JsonConvert.DeserializeObject<dto.endpoint.confirms.ConfirmsResponse>(updateData);
                        var epic = confirmData.epic;

                        if (!string.IsNullOrEmpty(epic))
                        {
                            SyncToDbConfirms(confirmData);

                            LogUpdateInfo(updateType, epic, (confirmData.size ?? 0).ToString(), confirmData.direction, confirmData.dealStatus);
                        }
                    }
                }

                void RegisterStreamEpic(StreamingStatusEnum status, string epic, EpicStreamListItem.EpicStreamListItemSource epicStreamListItemSource)
                {
                    var epicStreamListItem =
                        _iGApiEngine.EpicStreamList.Find(f => f.Epic == epic) ??
                        new EpicStreamListItem(epic, (EpicStreamListItem.EpicStreamListItemSource)epicStreamListItemSource);

                    if (status == StreamingStatusEnum.OPEN)
                        _iGApiEngine.AddEpicStreamListItem(epicStreamListItem);
                    else if (status == StreamingStatusEnum.CLOSED || status == StreamingStatusEnum.DELETED)
                        _iGApiEngine.RemoveEpicStreamListItem(epicStreamListItem);
                }

                void SyncToDbOpenPositions(LsTradeSubscriptionData openPositionData, string accountId)
                {
                    ApiDbContext apiDbContext = new();

                    if (
                        openPositionData.status == StreamingStatusEnum.OPEN ||
                        openPositionData.status == StreamingStatusEnum.UPDATED ||
                        openPositionData.status == StreamingStatusEnum.AMENDED)
                    {
                        apiDbContext.SaveOpenPosition(openPositionData, accountId);
                    }
                    else if (
                        openPositionData.status == StreamingStatusEnum.CLOSED ||
                        openPositionData.status == StreamingStatusEnum.DELETED)
                    {
                        var openPosition = Task.Run(async () => await apiDbContext.OpenPositions.FindAsync(accountId, openPositionData.dealId)).Result;

                        if (openPosition != null)
                            apiDbContext.Remove(openPosition);
                    }

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();
                }

                void SyncToDbWorkingOrders(
                    StreamingStatusEnum status,
                    dto.endpoint.workingorders.get.v2.WorkingOrderData WorkingOrderData,
                    string accountId)
                {
                    ApiDbContext apiDbContext = new();

                    if (
                        status == StreamingStatusEnum.OPEN ||
                        status == StreamingStatusEnum.UPDATED ||
                        status == StreamingStatusEnum.AMENDED)
                    {
                        apiDbContext.SaveWorkingOrder(WorkingOrderData, accountId);
                    }
                    else if (
                        status == StreamingStatusEnum.CLOSED ||
                        status == StreamingStatusEnum.DELETED)
                    {
                        var WorkingOrder = Task.Run(async () => await apiDbContext.WorkingOrders.FindAsync(accountId, WorkingOrderData.dealId)).Result;

                        if (WorkingOrder != null)
                            apiDbContext.Remove(WorkingOrder);
                    }

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();
                }

                void SyncToDbConfirms(dto.endpoint.confirms.ConfirmsResponse ConfirmData)
                {
                    ApiDbContext apiDbContext = new();

                    apiDbContext.SaveConfirmResponse(ConfirmData, isConsumable: false);

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();
                }
            }

            void LogUpdateInfo(UpdateType updateType, string epic, string size, string? direction, string? dealStatus)
            {
                WriteLog();
                WriteLog(Columns($"Trade activity received. {nameof(UpdateType)}: {updateType}"));
                WriteLog(Columns($"epic: {epic} size: {size} direction: {direction} dealStatus: {dealStatus ?? "UNKNOWN"}"));
                WriteLog();
            }
        }
    }
}
