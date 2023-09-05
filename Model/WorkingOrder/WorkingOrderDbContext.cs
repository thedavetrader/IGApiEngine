using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<WorkingOrder> WorkingOrders { get; set; }
        public static void WorkingOrderOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkingOrder>().Property(p => p.DealId).HasMaxLength(64);

            modelBuilder.Entity<WorkingOrder>()
                .HasKey(p => new
                {
                    p.AccountId,
                    p.DealId
                });
        }
    }
}