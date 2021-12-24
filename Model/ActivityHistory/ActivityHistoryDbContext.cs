using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<ActivityHistory>? ActivitiesHistory { get; set; }
        public static void ActivityOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityHistory>().Property(p => p.DealId).HasMaxLength(64);

            modelBuilder.Entity<ActivityHistory>()
                .HasKey(p => new
                {
                    p.Timestamp,
                    p.DealId
                });
        }
    }
}