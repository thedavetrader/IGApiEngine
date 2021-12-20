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
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails
            )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var currentAccount = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails);
            else
                currentAccount = iGApiDbContext.Accounts.Add(new Account(accountDetails)).Entity;

            return currentAccount;
        }

        public static Account? SaveAccount(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.auth.session.AccountInfo accountBalance
        )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var currentAccount = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails, accountBalance);
            else
                currentAccount = iGApiDbContext.Accounts.Add(new Account(accountDetails, accountBalance)).Entity;

            return currentAccount;
        }
        #endregion

        #region AccountBalance
        public static Account? SaveAccount(
           [NotNullAttribute] this IGApiDbContext iGApiDbContext,
           [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails
           )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var currentAccount = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails);
            else
                currentAccount = iGApiDbContext.Accounts.Add(new Account(accountDetails)).Entity;

            return currentAccount;
        }

        public static Account? SaveAccount(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountBalance accountBalance
        )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var currentAccount = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (currentAccount is not null)
                currentAccount.MapProperties(accountDetails, accountBalance);
            else
                currentAccount = iGApiDbContext.Accounts.Add(new Account(accountDetails, accountBalance)).Entity;

            return currentAccount;
        }
        #endregion

        #region StreamingAccountData
        public static Account? SaveAccount(
           [NotNullAttribute] this IGApiDbContext iGApiDbContext,
           [NotNullAttribute] IGWebApiClient.StreamingAccountData streamingAccountData,
           [NotNullAttribute] string accountId
           )
        {
            try
            {
                _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

                var currentAccount = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountId)).Result;

                if (currentAccount is not null)
                    currentAccount.MapProperties(streamingAccountData, accountId);
                else
                    currentAccount = iGApiDbContext.Accounts.Add(new Account(streamingAccountData, accountId)).Entity;

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