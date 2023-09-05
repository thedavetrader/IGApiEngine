using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class Account
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public Account()
        { AccountId = string.Format(Constants.InvalidEntry, nameof(Account)); }

        /// <summary>
        /// For creating new accounts using dto.endpoint.auth.session.
        /// </summary>
        /// <param name="accountDetails"></param>
        /// <param name="accountBalance"></param>
        public Account(
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.auth.session.AccountInfo accountBalance,
            bool isCurrent
            )
        {
            MapProperties(accountDetails, accountBalance, isCurrent);
            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
        }

        /// <summary>
        /// For creating new accounts using dto.endpoint.auth.session.
        /// </summary>
        /// <param name="accountDetails"></param>
        /// <param name="accountBalance"></param>
        public Account(
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails
            )
        {
            MapProperties(accountDetails);
            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
        }

        /// <summary>
        /// For creating new accounts using dto.endpoint.accountbalance
        /// </summary>
        /// <param name="accountDetails"></param>
        public Account(
            [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails
            )
        {
            MapProperties(accountDetails);
            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
        }

        /// <summary>
        /// For creating new accounts using dto.endpoint.accountbalance
        /// </summary>
        /// <param name="accountDetails"></param>
        /// <param name="accountBalance"></param>
        public Account(
            [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountBalance accountBalance
            )
        {
            MapProperties(accountDetails, accountBalance);
            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
        }

        /// <summary>
        /// For creating new accounts using IGWebApiClient.StreamingAccountData
        /// </summary>
        /// <param name="streamingAccountData"></param>
        /// <param name="accountId"></param>
        public Account([NotNullAttribute] IGWebApiClient.StreamingAccountData streamingAccountData, [NotNullAttribute] string accountId)
        {
            MapProperties(streamingAccountData, accountId);
            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
        }
    }
}

