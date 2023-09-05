using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    using static Log;
    internal static partial class DtoModelExtensions
    {
        #region Session
        public static Account? SaveAccount(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails
            )
        {
            var currentAccount = Task.Run(async () => await apiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails);
            else
                currentAccount = apiDbContext.Accounts.Add(new Account(accountDetails)).Entity;

            return currentAccount;
        }

        public static Account? SaveAccount(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.auth.session.AccountInfo accountBalance,
            bool isCurrent
        )
        {
            var currentAccount = Task.Run(async () => await apiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails, accountBalance, isCurrent);
            else
                currentAccount = apiDbContext.Accounts.Add(new Account(accountDetails, accountBalance, isCurrent)).Entity;

            return currentAccount;
        }
        #endregion

        #region AccountBalance
        public static Account? SaveAccount(
           [NotNullAttribute] this ApiDbContext apiDbContext,
           [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails
           )
        {
            var currentAccount = Task.Run(async () => await apiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails);
            else
                currentAccount = apiDbContext.Accounts.Add(new Account(accountDetails)).Entity;

            return currentAccount;
        }

        public static Account? SaveAccount(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountBalance accountBalance
        )
        {
            var currentAccount = Task.Run(async () => await apiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails, accountBalance);
            else
                currentAccount = apiDbContext.Accounts.Add(new Account(accountDetails, accountBalance)).Entity;

            return currentAccount;
        }
        #endregion

        #region StreamingAccountData
        public static Account? SaveAccount(
           [NotNullAttribute] this ApiDbContext apiDbContext,
           [NotNullAttribute] IGWebApiClient.StreamingAccountData streamingAccountData,
           [NotNullAttribute] string accountId
           )
        {
            try
            {
                var currentAccount = Task.Run(async () => await apiDbContext.Accounts.FindAsync(accountId)).Result;

                if (currentAccount is not null)
                    currentAccount.MapProperties(streamingAccountData, accountId);
                else
                    currentAccount = apiDbContext.Accounts.Add(new Account(streamingAccountData, accountId)).Entity;

                return currentAccount;
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
        }
        #endregion
    }
}