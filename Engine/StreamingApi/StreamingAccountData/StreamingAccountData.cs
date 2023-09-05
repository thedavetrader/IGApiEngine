using System.Diagnostics;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Lightstreamer.DotNet.Client;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private SubscribedTableKey? _accountBalanceStk = null;
        private AccountDetailsTableListerner? _streamingAccountData;

        private void SubscribeToAccountDetails()
        {
            try
            {
                if (!IsLoggedIn && !_cancellationToken.IsCancellationRequested) throw new IGApiConncectionError();

                _streamingAccountData = new AccountDetailsTableListerner();
                _streamingAccountData.Update += StreamingAccountDataUpdate;

                WriteLog(Columns("Lightstreamer - Subscribing to Account Details"));
                _accountBalanceStk = _iGStreamApiClient.SubscribeToAccountDetails(LoginSessionInformation.currentAccountId, _streamingAccountData);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }

        private void StreamingAccountDataUpdate(object? sender, UpdateArgs<StreamingAccountData> e)
        {
            try
            {
                if (!IsLoggedIn && !_cancellationToken.IsCancellationRequested) throw new IGApiConncectionError();

                var streamingAccountData = e.UpdateData;
                string currentAccountId = LoginSessionInformation.currentAccountId;

                using ApiDbContext apiDbContext = new();
                
                apiDbContext.SaveAccount(streamingAccountData, currentAccountId);
                
                Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait(); // Use wait, avoiding object disposed DbContext operations are still running.
            }
            catch (Exception ex) 
            {
                WriteException(ex);
                throw;
            }
        }

        private void UnsubscribeFromAccountDetails()
        {
            if ((_accountBalanceStk is not null) && (_iGStreamApiClient is not null))
            {
                _iGStreamApiClient.UnsubscribeTableKey(_accountBalanceStk);

                WriteLog(Columns("Successfully unsubscribed from account subscription."));
            }
        }
    }
}
