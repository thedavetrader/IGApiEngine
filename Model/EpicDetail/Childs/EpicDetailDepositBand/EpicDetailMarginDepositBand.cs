using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("epic_detail_margin_deposit_band")]
    public partial class EpicDetailMarginDepositBand
    {
        [Column("epic")]
        public string Epic { get; set; }

        [Column("currency")]
        public string Currency { get; set; }

        [Column("min")]
        public decimal Min { get; set; }

        [Column("max")]
        public decimal? Max { get; set; }

        [Column("margin")]
        public decimal? Margin { get; set; }

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

        // Navigation property.
        public EpicDetail EpicDetail { get; set; }
    }
}

