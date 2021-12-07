using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("epic_detail_opening_hour")]
    public partial class EpicDetailOpeningHour
	{
        [Column("epic")]
        public string Epic { get; set; }

		[Column("open_time")]
		public string OpenTime { get; set; }

		[Column("close_time")]
		public string CloseTime { get; set; }

		[Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

		// Navigation property.
		public EpicDetail EpicDetail { get; set; }
	}
}

