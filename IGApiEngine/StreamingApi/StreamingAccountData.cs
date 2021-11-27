using System.Diagnostics;
using IGApi.Model;
using IGApiEngine.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;

namespace IGApi
{
    public sealed partial class IGApiEngine
    {
        private SubscribedTableKey? _accountBalanceStk;
        private AccountDetailsTableListerner? _streamingAccountData;

        private void SubscribeToAccountDetails()
        {
            try
            {
                _ = LoginSessionInformation ?? throw new ArgumentNullException(nameof(LoginSessionInformation));

                _streamingAccountData = new AccountDetailsTableListerner();
                _streamingAccountData.Update += StreamingAccountDataUpdate;

                Log.WriteLine("Lightstreamer - Subscribing to Account Details");
                _accountBalanceStk = _iGStreamApiClient.SubscribeToAccountDetails(LoginSessionInformation.currentAccountId, _streamingAccountData);
            }
            catch (Exception ex)
            {
                Log.WriteLine("ApplicationViewModel - SubscribeToAccountDetails" + ex.Message);
            }
        }

        private void StreamingAccountDataUpdate(object? sender, UpdateArgs<StreamingAccountData> e)
        {
            _ = LoginSessionInformation ?? throw new ArgumentNullException(nameof(LoginSessionInformation));

            var streamingAccountData = e.UpdateData;
            string currentAccountId = LoginSessionInformation.currentAccountId;

            using IGApiDbContext iGApiDbContext = new();

            Task.Run(async () => await new IGApiStreamingAccountData(streamingAccountData, currentAccountId).InsertOrUpdateAsync());

        }

        private void UnsubscribeFromAccountDetails()
        {
            if ((_accountBalanceStk is not null) && (_iGStreamApiClient is not null))
            {
                _iGStreamApiClient.UnsubscribeTableKey(_accountBalanceStk);

                Log.WriteLine("Successfully unsubscribed from Account Balance Subscription");
            }
        }
    }
}
