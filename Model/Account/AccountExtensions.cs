using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        #region Session
        public static IGApiDbContext SaveAccount(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails
            )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var account = Task.Run(async ()=> await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (account is not null)
                account.MapProperties(accountDetails);
            else
                iGApiDbContext.Accounts.Add(new Account(accountDetails));

            return iGApiDbContext;
        }

        public static IGApiDbContext SaveAccount(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.auth.session.AccountInfo accountBalance
        )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var account = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (account is not null)
                account.MapProperties(accountDetails, accountBalance);
            else
                iGApiDbContext.Accounts.Add(new Account(accountDetails, accountBalance));

            return iGApiDbContext;
        }
        #endregion

        #region AccountBalance
        public static IGApiDbContext SaveAccount(
           [NotNullAttribute] this IGApiDbContext iGApiDbContext,
           [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails            
           )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var account = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (account is not null)
                account.MapProperties(accountDetails);
            else
                iGApiDbContext.Accounts.Add(new Account(accountDetails));

            return iGApiDbContext;
        }

        public static IGApiDbContext SaveAccount(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountBalance accountBalance
        )
        {
            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            var account = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountDetails.accountId)).Result;

            if (account is not null)
                account.MapProperties(accountDetails, accountBalance);
            else
                iGApiDbContext.Accounts.Add(new Account(accountDetails, accountBalance));

            return iGApiDbContext;
        }
        #endregion

        #region StreamingAccountData
        public static IGApiDbContext SaveAccount(
           [NotNullAttribute] this IGApiDbContext iGApiDbContext,
           [NotNullAttribute] IGWebApiClient.StreamingAccountData streamingAccountData, 
           [NotNullAttribute] string accountId
           )
        {
            try
            {
                _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

                var account = Task.Run(async () => await iGApiDbContext.Accounts.FindAsync(accountId)).Result;

                if (account is not null)
                    account.MapProperties(streamingAccountData, accountId);
                else
                    iGApiDbContext.Accounts.Add(new Account(streamingAccountData, accountId));

                return iGApiDbContext;
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(SaveAccount));
                throw;
            }
        }
        #endregion
    }
}