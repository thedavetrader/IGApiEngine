using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public class IGApiAccountBalance
    {
        /// <summary>
        /// This constructor allows to iniitialize IGApiAccountBalance basesd on the session AccountDetails from IGWebApiClient.dto.
        /// </summary>
        /// <param name="IGApiAccountBalance"></param>
        public IGApiAccountBalance(dto.endpoint.auth.session.AccountInfo accountInfo, [NotNullAttribute] string currentAccountId)
        {
            AccountId = currentAccountId;
            Balance = accountInfo.balance;
            Deposit = accountInfo.deposit;
            ProfitLoss = accountInfo.profitLoss;
            Available = accountInfo.available;
        }

        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public IGApiAccountBalance() { }

        public string AccountId { get; set; } = "[NULL]";
        public decimal? Balance { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? ProfitLoss { get; set; }
        public decimal? Available { get; set; }
    }
}
