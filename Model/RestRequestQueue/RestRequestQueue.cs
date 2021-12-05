using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("rest_request_queue")]
    public partial class RestRequestQueue
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Column("rest_request")]
        public string RestRequest { get; set; } = "[ERROR] RestRequest is required.";
        
        [Column("parameter")]
        public string? Parameters { get; set; } = string.Empty;
        
        [Column("execute_asap")]
        public bool ExecuteAsap { get; set; }
        
        [Column("is_recurrent")]
        public bool IsRecurrent { get; set; }
    }
}