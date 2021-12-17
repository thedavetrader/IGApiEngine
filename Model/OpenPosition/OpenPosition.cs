using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Model
{
    [Table("open_position")]
    public partial class OpenPosition
    {
		[Column("account_id")]
		public string AccountId { get; set; }
		
		[Column("epic")]
		public string Epic { get; set; } = Constants.PropertyNullError;
		
		[Column("deal_id")]
		public string DealId { get; set; }

		[Column("deal_reference")]
		public string DealReference { get; set; }

		[Column("created_date_utc")]
		public DateTime CreatedDateUtc { get; set; }
		
		[Column ("contract_size")]
		public decimal? ContractSize { get; set; }
		
		[Column("size")]
		public decimal? Size { get; set; }
		
		[Column("direction")]
		public string Direction { get; set; } = Constants.PropertyNullError;
		
		[Column("limit_level")]
		public decimal? LimitLevel { get; set; }
		
		[Column("level")]
		public decimal? Level { get; set; }
		
		[Column("currency")]
		public string Currency { get; set; } = Constants.PropertyNullError;
		
		[Column("controlled_risk")]
		public bool ControlledRisk { get; set; }
		
		[Column("stop_level")]
		public decimal? StopLevel { get; set; }
		
		[Column("trailing_step")]
		public decimal? TrailingStep { get; set; }
		
		[Column("trailing_stop_distance")]
		public decimal? TrailingStopDistance { get; set; }

		[Column("api_last_update")]
		public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;
	}
}
