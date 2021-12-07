using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("epic_detail_currency")]
    public partial class EpicDetailCurrency
    {
        [Column("epic")]
        public string Epic { get; set; }

		[Column("code")]
		public string Code { get; set; }

		[Column("symbol")]
		public string? Symbol { get; set; }

		[Column("base_exchange_rate")]
		public decimal? BaseExchangeRate { get; set; }

		[Column("is_default")]
		public bool IsDefault { get; set; }

		[Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

		// Navigation property.
		public EpicDetail EpicDetail { get; set; }
	}
}

