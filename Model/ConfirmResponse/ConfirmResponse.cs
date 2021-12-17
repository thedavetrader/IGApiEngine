using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("confirm_response")]
    public partial class ConfirmResponse
    {
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Column("status")]
        public string? Status { get; set; }
        
        [Column("reason")]
        public string? Reason { get; set; }
        
        [Column("deal_status")]
        public string? DealStatus { get; set; }
        
        [Column("epic")]
        public string? Epic { get; set; }
        
        [Column("expiry")]
        public string? Expiry { get; set; }
        
        [Column("deal_reference")]
        public string DealReference { get; set; }
        
        [Column("deal_id")]
        public string DealId { get; set; }
        
        [Column("level")]
        public decimal? Level { get; set; }
        
        [Column("size")]
        public decimal? Size { get; set; }
        
        [Column("direction")]
        public string? Direction { get; set; }
        
        [Column("stop_level")]
        public decimal? StopLevel { get; set; }
        
        [Column("limit_level")]
        public decimal? LimitLevel { get; set; }
        
        [Column("stop_distance")]
        public decimal? StopDistance { get; set; }
        
        [Column("limit_distance")]
        public decimal? LimitDistance { get; set; }
        
        [Column("guaranteed_stop")]
        public bool GuaranteedStop { get; set; }

        //TODO:     ZEROPRIO For now store affected_deals as json, until its purpose is clear.
        [Column("affected_deals")]
        public string? AffectedDeals { get; set; }
    }
}
