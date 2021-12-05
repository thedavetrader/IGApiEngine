using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("account")]
    public partial class Account
    {
        #region AccountDetailsProperties
        [Column("api_last_update")]
        
        public DateTime ApiLastUpdate { get; set; } = DateTime.Now;
        [Column("account_id")]
        
        public string AccountId { get; set; }
        [Column("account_name")]
        
        public string? AccountName { get; set; }
        [Column("account_alias")]
        
        public string? AccountAlias { get; set; }
        [Column("status")]
        
        public string? Status { get; set; }
        [Column("account_type")]
        
        public string? AccountType { get; set; }
        [Column("preferred")]
        
        public bool? Preferred { get; set; }
        [Column("currency")]
        
        public string? Currency { get; set; }
        [Column("can_transfer_from")]
        
        public bool? CanTransferFrom { get; set; }
        [Column("can_transfer_to")]
        
        public bool? CanTransferTo { get; set; }
        #endregion

        #region AccountBalanceProperties
        [Column("balance")]
        public decimal? Balance { get; set; }
        
        [Column("deposit")]
        public decimal? Deposit { get; set; }
        
        [Column("profit_and_loss")]
        public decimal? ProfitAndLoss { get; set; }
        
        [Column("equity")]
        public decimal? Equity { get; set; }
        
        [Column("equity_used")]
        public decimal? EquityUsed { get; set; }
        
        [Column("used_margin")]
        public decimal? UsedMargin { get; set; }
        
        [Column("amount_due")]
        public decimal? AmountDue { get; set; }
        
        [Column("available_cash")]
        public decimal? AvailableCash { get; set; }
        #endregion AccountBalance
    }
}

