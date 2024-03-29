﻿using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("api_engine_status")]
    public partial class ApiEngineStatus
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("is_alive")]
        public DateTime IsAlive{ get; set; }
    }
}

