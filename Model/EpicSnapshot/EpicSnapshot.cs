using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("epic_snapshot")]
    public partial class EpicSnapshot
    {
        [Column("resolution")]
        public string Resolution { get; set; }
        [Column("open_time")]
        public DateTime OpenTime { get; set; }
        [Column("close_time")]
        public DateTime CloseTime { get; set; }
        [Column("open_time_utc")]
        public DateTime OpenTimeUTC { get; set; }
        [Column("close_time_utc")]
        public DateTime CloseTimeUTC { get; set; }
        [Column("epic")]
        public string Epic { get; set; }
        [Column("open_bid")]
        public decimal? OpenBid { get; set; }
        [Column("open_ask")]
        public decimal? OpenAsk { get; set; }
        [Column("open_median")]
        public decimal? OpenMedian { get; set; }
        [Column("open_last_traded")]
        public decimal? OpenLastTraded { get; set; }
        [Column("close_bid")]
        public decimal? CloseBid { get; set; }
        [Column("close_ask")]
        public decimal? CloseAsk { get; set; }
        [Column("close_median")]
        public decimal? CloseMedian { get; set; }
        [Column("close_last_traded")]
        public decimal? CloseLastTraded { get; set; }
        [Column("high_bid")]
        public decimal? HighBid { get; set; }
        [Column("high_ask")]
        public decimal? HighAsk { get; set; }
        [Column("high_median")]
        public decimal? HighMedian { get; set; }
        [Column("high_last_traded")]
        public decimal? HighLastTraded { get; set; }
        [Column("low_bid")]
        public decimal? LowBid { get; set; }
        [Column("low_ask")]
        public decimal? LowAsk { get; set; }
        [Column("low_median")]
        public decimal? LowMedian { get; set; }
        [Column("daily_movement_percentage")]
        public decimal? DailyMovementPercentage { get; set; }
        [Column("low_last_traded")]
        public decimal? LowLastTraded { get; set; }
        [Column("last_traded_volume")]
        public decimal? lastTradedVolume { get; set; }

    }
}

