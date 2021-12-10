using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<WorkingOrder>? WorkingOrders { get; set; }
        public static void WorkingOrderOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkingOrder>()
                .HasKey(p => new
                {
                    p.AccountId,
                    p.DealId
                });
        }
    }
}