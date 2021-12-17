using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("transaction_history")]
    public partial class TransactionHistory
    {
        [Column("date")]
        public DateTime Date { get; set; }
        
        [Column("instrument_name")]
        public string InstrumentName { get; set; }
        
        [Column("period")]
        public string? Period { get; set; }
        
        [Column("profit_and_loss")]
        public decimal ProfitAndLoss { get; set; }
        
        [Column("transaction_type")]
        public string TransactionType { get; set; }
        
        [Column("reference")]
        public string Reference { get; set; }
        
        [Column("open_level")]
        public decimal? OpenLevel { get; set; }
        
        [Column("close_level")]
        public decimal? CloseLevel { get; set; }
        
        [Column("size")]
        public decimal? Size { get; set; }
        
        [Column("currency")]
        public string Currency { get; set; }
        
        [Column("cash_transaction")]
        public bool CashTransaction { get; set; }
    }
}
