using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("base_currency_convert_ratio")]
    public partial class BaseCurrencyConvertRatio
    {
        [Column("epic")]
        public string Epic { get; set; }

        [Column("base_exchange_rate")]
        public decimal? BaseExchangeRate { get; set; }
    }
}

