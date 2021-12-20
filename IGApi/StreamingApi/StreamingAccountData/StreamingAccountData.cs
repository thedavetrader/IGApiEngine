﻿using System.Diagnostics;
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
                if (!IsLoggedIn) throw new IGApiConncectionError();

                _streamingAccountData = new AccountDetailsTableListerner();
                _streamingAccountData.Update += StreamingAccountDataUpdate;

                WriteLog(Messages("Lightstreamer - Subscribing to Account Details"));
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
                if (!IsLoggedIn) throw new IGApiConncectionError();

                var streamingAccountData = e.UpdateData;
                string currentAccountId = LoginSessionInformation.currentAccountId;

                using IGApiDbContext iGApiDbContext = new();
                
                iGApiDbContext.SaveAccount(streamingAccountData, currentAccountId);
                
                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait(); // Use wait, avoiding object disposed DbContext operations are still running.
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

                WriteLog(Messages("Successfully unsubscribed from Account Balance Subscription"));
            }
        }
    }
}
