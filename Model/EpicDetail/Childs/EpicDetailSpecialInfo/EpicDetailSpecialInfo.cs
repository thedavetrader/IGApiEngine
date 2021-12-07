using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    [Table("epic_detail_special_info")]
    public partial class EpicDetailSpecialInfo
    {
        [Column("epic")]
        public string Epic { get; set; }

        [Column("special_info")]
        public string SpecialInfo { get; set; }

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;
        
        // Navigation property.
        public EpicDetail EpicDetail { get; set; }
    }
}

