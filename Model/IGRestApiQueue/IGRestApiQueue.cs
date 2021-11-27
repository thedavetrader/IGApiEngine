using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGApi.Model
{
    public class IGRestApiQueue
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string RestRequest { get; set; } = "[ERROR] RestRequest is required.";
        public string? Parameters { get; set; } = string.Empty;
        public bool ExecuteAsap { get; set; }
        public bool IsRecurrent { get; set; }
    }
}