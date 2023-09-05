using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicSnapshotAllowance> EpicSnapshotAllowances { get; set; }
        public static void EpicSnapshotAllowanceOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicSnapshotAllowance>().Property(p => p.AllowanceExpiryDate).HasComputedColumnSql("dateadd(second, allowance_expiry, getdate())");

            modelBuilder.Entity<EpicSnapshotAllowance>()
                .HasKey(p => new
                {
                    // Just a fake key so that the object can be tracked.
                    p.AllowanceExpiry,
                    p.RemainingAllowance,
                    p.TotalAllowance
                });
        }
    }
}
