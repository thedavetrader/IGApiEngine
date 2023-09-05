using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("market_node")]
    public partial class MarketNode
    {
        [Column("market_node_id")]
        public string MarketNodeId { get; set; }
        
        [Column("parent_market_node_id")]
        public string? ParentMarketNodeId { get; set; }
        
        [Column("name")]
        public string Name { get; set; }

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

        [Column("is_browsed")]
        public bool IsBrowsed { get; set; }
    }
}

