using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicDetail> EpicDetails { get; set; }
        public static void EpicDetailOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicDetail>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetail>().Property(p => p.Spread).HasComputedColumnSql("offer - bid", stored: true);
            modelBuilder.Entity<EpicDetail>().Property(p => p.Median).HasComputedColumnSql("(offer + bid) / 2.0", stored: true);
            modelBuilder.Entity<EpicDetail>().Property(p => p.IsImportAutorized).HasDefaultValue(true);
            modelBuilder.Entity<EpicDetail>().Property(p => p.IsTimedOut).HasDefaultValue(false);

            modelBuilder.Entity<EpicDetail>()
                .HasKey(p => new
                {
                    p.Epic
                });
        }
    }
}