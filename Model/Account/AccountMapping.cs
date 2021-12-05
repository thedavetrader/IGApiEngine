using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class Account
    {
        #region Session
        public void MapProperties(
            [NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.auth.session.AccountInfo accountBalance
            )
        {
            MapProperties(accountDetails);
            Balance = accountBalance.balance ?? Balance;
            Deposit = accountBalance.deposit ?? Deposit;
            ProfitAndLoss = accountBalance.profitLoss ?? ProfitAndLoss;
            AvailableCash = accountBalance.available ?? AvailableCash;
            ApiLastUpdate = DateTime.Now;
        }

        public void MapProperties([NotNullAttribute] dto.endpoint.auth.session.AccountDetails accountDetails)
        {
            if (accountDetails.accountId is not null)
            {
                AccountId = accountDetails.accountId;
                AccountName = accountDetails.accountName ?? AccountName;
                Preferred = accountDetails.preferred;
                AccountType = accountDetails.accountType ?? AccountType;
                ApiLastUpdate = DateTime.Now;
            }
            else
                throw new PrimaryKeyNullReferenceException(nameof(accountDetails.accountId));
        }
        #endregion

        #region AccountBalance
        public void MapProperties(
            [NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails,
            [NotNullAttribute] dto.endpoint.accountbalance.AccountBalance accountBalance
            )
        {
            MapProperties(accountDetails);
            Balance = accountBalance.balance ?? Balance;
            Deposit = accountBalance.deposit ?? Deposit;
            ProfitAndLoss = accountBalance.profitLoss ?? ProfitAndLoss;
            AvailableCash = accountBalance.available ?? AvailableCash;
            ApiLastUpdate = DateTime.Now;
        }

        public void MapProperties([NotNullAttribute] dto.endpoint.accountbalance.AccountDetails accountDetails)
        {
            if (accountDetails.accountId is not null)
            {
                AccountId = accountDetails.accountId;
                AccountName = accountDetails.accountName ?? AccountName;
                AccountAlias = accountDetails.accountAlias ?? AccountAlias;
                AccountType = accountDetails.accountType ?? AccountType;
                CanTransferFrom = accountDetails.canTransferFrom;
                CanTransferTo = accountDetails.canTransferTo;
                Currency = accountDetails.currency ?? Currency;
                Preferred = accountDetails.preferred;
                Status = accountDetails.status ?? Status;
                ApiLastUpdate = DateTime.Now;
            }
            else
                throw new PrimaryKeyNullReferenceException(nameof(accountDetails.accountId));
        }
        #endregion

        #region StreamingAccountData
        public void MapProperties(
            [NotNullAttribute] IGWebApiClient.StreamingAccountData streamingAccountData, 
            [NotNullAttribute] string accountId)
        {
                AccountId = accountId;
                Balance = streamingAccountData.Balance ?? Balance;
                Equity = streamingAccountData.Equity ?? Equity;
                EquityUsed = streamingAccountData.EquityUsed ?? EquityUsed;
                ProfitAndLoss = streamingAccountData.ProfitAndLoss ?? ProfitAndLoss;
                Deposit = streamingAccountData.Deposit ?? Deposit;
                UsedMargin = streamingAccountData.UsedMargin ?? UsedMargin;
                AmountDue = streamingAccountData.AmountDue ?? AmountDue;
                AvailableCash = streamingAccountData.AvailableCash ?? AvailableCash;
                ApiLastUpdate = DateTime.Now;
        }
        #endregion
    }
}

