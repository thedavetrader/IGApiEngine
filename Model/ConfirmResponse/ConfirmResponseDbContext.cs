using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<ConfirmResponse>? ConfirmResponses { get; set; }
        public static void ConfirmResponseOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfirmResponse>().IsMemoryOptimized(true);

            modelBuilder.Entity<ConfirmResponse>().Property(p => p.DealReference).HasMaxLength(30);
            modelBuilder.Entity<ConfirmResponse>().Property(p => p.DealId).HasMaxLength(64);

            modelBuilder.Entity<ConfirmResponse>()
                .HasKey(p => new
                {
                    p.Timestamp,
                    p.DealReference // Only one deal reference at a time can be added, modified or deleted. It is the caller responsibility to consume the confirmresponse.
                });
        }
    }
}