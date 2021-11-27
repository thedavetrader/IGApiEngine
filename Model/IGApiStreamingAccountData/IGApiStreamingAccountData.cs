using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public class IGApiStreamingAccountData
    {
        public string AccountId = "[NULL]";
        public decimal? Balance { get; set; }
        public decimal? Equity { get; set; }
        public decimal? EquityUsed { get; set; }
        public decimal? ProfitAndLoss { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? UsedMargin { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? AvailableCash { get; set; }
        /// <summary>
        /// This constructor allows to iniitialize IGApiStreamingAccountData basesd on the session StreamingAccountData from IGWebApiClient.dto.
        /// </summary>
        /// <param name="streamingAccountData"></param>
        public IGApiStreamingAccountData(IGWebApiClient.StreamingAccountData streamingAccountData, [NotNullAttribute] string currentAccountId) : base()
        {
            AccountId = currentAccountId;
            Balance = streamingAccountData.Balance;
            Equity = streamingAccountData.Equity;
            EquityUsed = streamingAccountData.EquityUsed;
            ProfitAndLoss = streamingAccountData.ProfitAndLoss;
            Deposit = streamingAccountData.Deposit;
            UsedMargin = streamingAccountData.UsedMargin;
            AmountDue = streamingAccountData.AmountDue;
            AvailableCash = streamingAccountData.AvailableCash;
        }

        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public IGApiStreamingAccountData() : base() { }

    }
}
