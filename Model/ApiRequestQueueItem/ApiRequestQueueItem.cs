using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("api_request_queue_item")]
    public partial class ApiRequestQueueItem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
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