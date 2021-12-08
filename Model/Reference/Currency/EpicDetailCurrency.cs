using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("currency")]
    public partial class Currency
    {
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

        #region Childs
        public virtual List<EpicDetailCurrency>? Currencies { get; set; }
        #endregion
    }
}

