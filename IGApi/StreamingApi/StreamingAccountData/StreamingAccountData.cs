using System.Diagnostics;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        private SubscribedTableKey? _accountBalanceStk = null;
        private AccountDetailsTableListerner? _streamingAccountData;
        private void SubscribeToAccountDetails()
        {
            try
            {
                _ = LoginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation));

                _streamingAccountData = new AccountDetailsTableListerner();
                _streamingAccountData.Update += StreamingAccountDataUpdate;

                Log.WriteLine("Lightstreamer - Subscribing to Account Details");
                _accountBalanceStk = _iGStreamApiClient.SubscribeToAccountDetails(LoginSessionInformation.currentAccountId, _streamingAccountData);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(SubscribeToAccountDetails));
                throw;
            }
        }

        private void StreamingAccountDataUpdate(object? sender, UpdateArgs<StreamingAccountData> e)
        {
            try
            {
                _ = LoginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation));

                var streamingAccountData = e.UpdateData;
                string currentAccountId = LoginSessionInformation.currentAccountId;

                using IGApiDbContext iGApiDbContext = new();
                
                iGApiDbContext.SaveAccount(streamingAccountData, currentAccountId);
                
                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait(); // Use wait, avoiding object disposed DbContext operations are still running.
            }
            catch (Exception ex) 
            {
                Log.WriteException(ex, nameof(StreamingAccountDataUpdate));
                throw;
            }
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
