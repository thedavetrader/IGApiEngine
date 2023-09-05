using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("epic_snapshot_allowance")]
    public partial class EpicSnapshotAllowance
    {
        [Column("allowance_expiry")]
		public int AllowanceExpiry { get; set; }
        [Column("remaining_allowance")]
		public int RemainingAllowance { get; set; }
        [Column("total_allowance")]
		public int TotalAllowance { get; set; }
        [Column("allowance_expiry_date")]
        public DateTime AllowanceExpiryDate { get; set; }
    }
}

