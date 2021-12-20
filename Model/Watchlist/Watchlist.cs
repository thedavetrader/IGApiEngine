using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("watchlist")]
    public partial class Watchlist
    {
		[Column("account_id")]
		public string AccountId { get; set; }

		[Column("id")]
		public string Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("editable")]
		public bool Editable { get; set; }

		[Column("deletable")]
		public bool Deleteable { get; set; }

		[Column("default_system_watchlist")]
		public bool DefaultSystemWatchlist { get; set; }

		[Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;
    }
}
