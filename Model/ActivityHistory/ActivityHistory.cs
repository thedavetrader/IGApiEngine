using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Model
{
	[Table("activity_history")]
	public partial class ActivityHistory
	{
		[Column("timestamp")]
		public DateTime Timestamp { get; set; }

		[Column("epic")]
		public string Epic { get; set; }

		[Column("period")]
		public string? Period { get; set; }

		[Column("deal_id")]
		public string DealId { get; set; }

		[Column("channel")]
		public string Channel { get; set; }

		[Column("type")]
		public string Type { get; set; }

		[Column("status")]
		public string Status { get; set; }

		[Column("descpription")]
		public string Description { get; set; }

		[Column("reference")]
		public string reference { get; set; }

		[Column("deal_reference")]
		public string? DealReference { get; set; } 
		
		[Column("market_name")]
        public string? MarketName { get; set; }
		
		[Column("good_till_date")]
        public string? GoodTillDate { get; set; }
		
		[Column("currency")]
        public string? Currency { get; set; }
		
		[Column("size")]
        public decimal? Size { get; set; }
		
		[Column("direction")]
        public string? Direction { get; set; }
		
		[Column("level")]
        public decimal? Level { get; set; }
		
		[Column("stop_level")]
        public decimal? StopLevel { get; set; }
		
		[Column("stop_distance")]
		public decimal? StopDistance { get; set; }
		
		[Column("guaranteed_stop")] 
		public bool GuaranteedStop { get; set; }
		
		[Column("trailing_stop_distance")] 
		public decimal? TrailingStopDistance { get; set; }
		
		[Column("trailing_step")] 
		public decimal? TrailingStep { get; set; }
		
		[Column("limit_level")] 
		public decimal? LimitLevel { get; set; }
		
		[Column("limit_distance")] 
		public decimal? LimitDistance{ get; set; }
	}
}
