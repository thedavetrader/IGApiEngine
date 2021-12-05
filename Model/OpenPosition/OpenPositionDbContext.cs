using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<OpenPosition>? OpenPositions { get; set; }
        public static void OpenPositionOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OpenPosition>()
                .HasKey(p => new
                {
                    p.AccountId,
                    p.DealId
                });
        }
    }
}