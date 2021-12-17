using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<EpicTick>? EpicTicks { get; set; }
        public static void EpicTickOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicTick>().IsMemoryOptimized(true);

            modelBuilder.Entity<EpicTick>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicTick>()
                .HasKey(p => new
                {
                    p.Epic
                });
        }
    }
}
