using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("watchlist_epic_detail")]
    public partial class WatchlistEpicDetail
    {
		[Column("account_id")]
		public string AccountId { get; set; }

		[Column("watchlist_id")]
		public string WatchlistId { get; set; }

        [Column("epic")]
        public string Epic { get; set; }

        #region Relations
        public Watchlist Watchlist { get; set; }
        public EpicDetail EpicDetail { get; set; }
        #endregion
    }
}
