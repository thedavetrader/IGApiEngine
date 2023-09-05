using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("api_event_handler")]
    public partial class ApiEventHandler
    {
        [Column("sender")]       
        public string Sender { get; set; } 
        
        //  The stored proc name to the event_handler that will be queued.
        [Column("delegate")]       
        public string Delegate { get; set; }

        [Column("priority")]
        public int Priority { get; set; }
    }
}

