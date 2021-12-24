using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("search_result")]
    public partial class SearchResult
    {
        [Column("epic")]
        public string Epic { get; set; }
    }
}
