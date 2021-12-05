using System.Globalization;
using System.Text;
using dto.endpoint.positions.get.otc.v1;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;
using Newtonsoft.Json;

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
            _tradeSubscribedTableKey = null; //TODO: Important to initialise to null (?According to IGWebApi-sample!).
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

                Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTradeData]", "", "", ""));
                Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTradeData]", "", "", "Subscribing Trades to Trade Lightning streamer."));
                Log.WriteLine(string.Format(CultureInfo.InvariantCulture, Log.FormatString, "[StreamingTradeData]", "", "", ""));

                if (!string.IsNullOrEmpty(LoginSessionInformation.lightstreamerEndpoint))
                {
                    _tradeSubscribedTableKey = _iGStreamApiClient.SubscribeToTradeSubscription(LoginSessionInformation.currentAccountId, _tradeSubscription);
                    Log.WriteLine("Successfully subscribed to confirms(CONFIRMS), working order updates(WOU) and open position updates(OPU)");
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
                    var confirms = update.GetNewValue("CONFIRMS");
                    var opu = update.GetNewValue("OPU");
                    var wou = update.GetNewValue("WOU");

                    string? inputData = null;

                    if (!(String.IsNullOrEmpty(opu)))
                    {
                        inputData = opu;
                        Log.WriteLine("OPU");
                    }
                    if (!(String.IsNullOrEmpty(wou)))
                    {
                        inputData = wou;
                        Log.WriteLine("WOU");
                    }
                    if (!(String.IsNullOrEmpty(confirms)))
                    {
                        inputData = confirms;
                        Log.WriteLine("CONFIRMS");
                    }

                    if (!(String.IsNullOrEmpty(inputData)))
                    {
                        var updateData = JsonConvert.DeserializeObject<LsTradeSubscriptionData>(inputData);
                        var accountId = itemName.Replace("TRADE:", "");
                        var status = updateData.status;
                        var epic = updateData.epic;
                        var dealId = updateData.dealId;

                        using IGApiDbContext iGApiDbContext = new();
                        _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));

                        #region SycnWithDb
                        if (status == StreamingStatusEnum.OPEN || status == StreamingStatusEnum.UPDATED || status == StreamingStatusEnum.AMENDED)
                        {
                            Task.Run(async () => await iGApiDbContext.SaveOpenPosition(updateData, accountId).SaveChangesAsync()).Wait();
                        }
                        else if (status == StreamingStatusEnum.CLOSED || status == StreamingStatusEnum.DELETED)
                        {
                            var openPosition = Task.Run(async () => await iGApiDbContext.OpenPositions.FindAsync(accountId, dealId)).Result;

                            if (openPosition != null)
                                iGApiDbContext.Remove(openPosition);

                            Task.Run(async () => await iGApiDbContext.SaveChangesAsync());
                        }
                        #endregion

                        #region RegisterStreamEpic

                        //TODO CHeck for invalid Epic (streamingprices available). Need Epic meta data first.
                        // if not invalid epic:
                        //{
                        if (status == StreamingStatusEnum.OPEN)
                            _iGApiEngine.SubscribeEpicTickDetails(new EpicStreamListItem(epic));
                        else if (status == StreamingStatusEnum.CLOSED || status == StreamingStatusEnum.DELETED)
                            _iGApiEngine.UnsubscribeEpicTickDetails(new EpicStreamListItem(epic));
                        //}
                        //else
                        //LogInvalidEpic(epic)
                        #endregion
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
