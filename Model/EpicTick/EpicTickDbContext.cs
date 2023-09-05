using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicTick> EpicTicks { get; set; }
        public static void EpicTickOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicTick>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicTick>().Property(p => p.Spread).HasComputedColumnSql("offer - bid", stored:true);
            modelBuilder.Entity<EpicTick>().Property(p => p.Median).HasComputedColumnSql("(offer + bid) / 2.0", stored:true);
            modelBuilder.Entity<EpicTick>().ToTable(t => t.IsMemoryOptimized());
            modelBuilder.Entity<EpicTick>()
                .HasKey(p => new
                {
                    p.Epic
                });
        }
    }
}
