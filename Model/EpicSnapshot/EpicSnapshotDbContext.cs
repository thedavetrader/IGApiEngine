using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicSnapshot> EpicSnapshots { get; set; }
        public static void EpicSnapshotOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicSnapshot>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicSnapshot>().Property(p => p.OpenMedian).HasComputedColumnSql("(open_ask + open_bid) / 2.0", stored: true);
            modelBuilder.Entity<EpicSnapshot>().Property(p => p.CloseMedian).HasComputedColumnSql("(close_ask + close_bid) / 2.0", stored: true);
            modelBuilder.Entity<EpicSnapshot>().Property(p => p.HighMedian).HasComputedColumnSql("(high_ask + high_bid) / 2.0", stored: true);
            modelBuilder.Entity<EpicSnapshot>().Property(p => p.LowMedian).HasComputedColumnSql("(low_ask + low_bid) / 2.0", stored: true);
            modelBuilder.Entity<EpicSnapshot>().Property(p => p.DailyMovementPercentage)
                .HasComputedColumnSql(
                "(100 * (((high_ask + high_bid) / 2.0) - ((low_ask + low_bid) / 2.0)) / nullif(((low_ask + low_bid) / 2.0), 0))"
                , stored: true);

            modelBuilder.Entity<EpicSnapshot>()
                .HasKey(p => new
                {
                    p.Resolution,
                    p.OpenTimeUTC,
                    p.Epic
                });
            modelBuilder.Entity<EpicSnapshot>()
                .HasIndex(p => new
                {
                    p.Resolution,
                    p.OpenTime,
                    p.Epic
                });
        }
    }
}
