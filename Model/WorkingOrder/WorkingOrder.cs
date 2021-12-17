using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Model
{
    [Table("working_order")]
    public partial class WorkingOrder
    {
        [Column("account_id")]
        public string AccountId { get; set; }

        [Column("deal_id")]
        public string DealId { get; set; }

        [Column("direction")]
        public string Direction { get; set; }

        [Column("epic")]
        public string Epic { get; set; }

        [Column("order_size")]
        public decimal? OrderSize { get; set; }

        [Column("order_level")]
        public decimal? OrderLevel { get; set; }

        [Column("time_in_force")]
        public string TimeInForce { get; set; }

        [Column("good_till_date")]
        public DateTime? GoodTillDate { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("guaranteed_stop")]
        public bool GuaranteedStop { get; set; }

        [Column("order_type")]
        public string OrderType { get; set; }

        [Column("stop_distance")]
        public decimal? StopDistance { get; set; }

        [Column("limit_distance")]
        public decimal? LimitDistance { get; set; }

        [Column("currency_code")]
        public string? CurrencyCode { get; set; }

        [Column("dma")]
        public bool Dma { get; set; }

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;
    }
}
