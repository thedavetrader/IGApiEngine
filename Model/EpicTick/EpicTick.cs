﻿using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("epic_tick")]
    public partial class EpicTick
    {
        [Column("epic")]
        public string Epic { get; set; }
        
        [Column("mid_open")]
        public decimal? MidOpen{ get; set; }
        
        [Column("high")]
        public decimal? High{ get; set; }
        
        [Column("low")]
        public decimal? Low{ get; set; }
        
        [Column("change")]
        public decimal? Change{ get; set; }
        
        [Column("change_percentage")]
        public decimal? ChangePct{ get; set; }
        
        [Column("update_time")]
        public DateTime UpdateTime{ get; set; } = DateTime.UtcNow;
        
        [Column("market_delay")]
        public int? MarketDelay{ get; set; }
        
        [Column("market_state")]
        public string MarketState { get; set; }
        
        [Column("bid")]
        public decimal? Bid{ get; set; }
        
        [Column("offer")]
        public decimal? Offer{ get; set; }

        [Column("spread")]
        public decimal? Spread { get; set; }
        
        [Column("median")]
        public decimal? Median { get; set; }
    }
}

