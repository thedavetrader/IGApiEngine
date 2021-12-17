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
		[Column("epic")]
		public string Epic { get; set; }

		[Column("deal_id")]
		public string DealId { get; set; }

		[Column("activity_history_id")]
		public string ActivityHistoryId { get; set; }

		[Column("timestamp")]
		public DateTime Timestamp { get; set; }

		[Column("activity")]
		public string Activity { get; set; }

		[Column("market_name")]
		public string MarketName { get; set; }

		[Column("period")]
		public string? Period { get; set; }

		[Column("result")]
		public string Result { get; set; }

		[Column("channel")]
		public string Channel { get; set; }

		[Column("currency")]
		public string Currency { get; set; }

		[Column("size")]
		public string Size { get; set; }

		[Column("level")]
		public decimal? Level { get; set; }

		[Column("stop")]
		public decimal? Stop { get; set; }

		[Column("stop_type")]
		public string? StopType { get; set; }

		[Column("limit")]
		public decimal? Limit { get; set; }

		[Column("action_status")]
		public string ActionStatus { get; set; }

		[Column("reference")]
		public string Reference { get; set; }
	}
}
