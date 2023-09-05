using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<OpenPosition> OpenPositions { get; set; }
        public static void OpenPositionOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OpenPosition>().Property(p => p.DealId).HasMaxLength(64);

            modelBuilder.Entity<OpenPosition>()
                .HasKey(p => new
                {
                    p.AccountId,
                    p.DealId
                });
        }
    }
}