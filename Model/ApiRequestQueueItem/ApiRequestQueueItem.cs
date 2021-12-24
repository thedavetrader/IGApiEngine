using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("api_request_queue_item")]
    public partial class ApiRequestQueueItem
    {
        [Column("guid")]
        public Guid Guid { get; set; }

        [Column("parent_guid")]
        public Guid? ParentGuid { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [Column("request")]
        public string Request { get; set; } = "[ERROR] RestRequest is required.";
        
        [Column("parameter")]
        public string? Parameters { get; set; } = string.Empty;
        
        [Column("execute_asap")]
        public bool ExecuteAsap { get; set; }
        
        [Column("is_recurrent")]
        public bool IsRecurrent { get; set; }

        [Column("is_running")]
        public bool IsRunning { get; set; }
    }
}