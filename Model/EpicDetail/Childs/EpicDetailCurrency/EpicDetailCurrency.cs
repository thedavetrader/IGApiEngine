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

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

		// Navigation property.
		public EpicDetail EpicDetail { get; set; }
        public Currency Currency { get; set; }
    }
}

