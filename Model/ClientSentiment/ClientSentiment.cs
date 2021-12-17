using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("client_sentiment")]
    public partial class ClientSentiment
    {
		[Column("market_id")]
		public string MarketId { get; set; }

		[Column("long_position_percentage")]
		public decimal? LongPositionPercentage { get; set; }

		[Column("short_position_percentage")]
		public decimal? ShortPositionPercentage { get; set; }

		[Column("api_last_update")]
		public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;
	}
}

