using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("epic_detail_snapshot")]
    public partial class EpicDetailSnapshot
    {
        [Column("epic")]
        public string Epic { get; set; }

        [Column("market_status")]
        public string MarketStatus { get; set; }
        [Column("update_time")]
        public string UpdateTime { get; set; }
        [Column("offer")]
        public decimal? Offer { get; set; }
        [Column("bid")]
        public decimal? Bid { get; set; }
        [Column("low")]
        public decimal? Low { get; set; }
        [Column("high")]
        public decimal? High { get; set; }
        [Column("percentage_change")]
        public decimal? PercentageChange { get; set; }
        [Column("net_change")]
        public decimal? NetChange { get; set; }
        [Column("binary_odds")]
        public decimal? BinaryOdds { get; set; }
        [Column("scaling_factor")]
        public int DecimalPlacesFactor { get; set; }
        [Column("controlled_risk_extra_spread")]
        public int ScalingFactor { get; set; }
        [Column("decimal_places_factor")]
        public decimal? ControlledRiskExtraSpread { get; set; }
        [Column("delay_time")]
        public int DelayTime { get; set; }

        // Navigation property.
        public EpicDetail EpicDetail { get; set; }

    }
}

