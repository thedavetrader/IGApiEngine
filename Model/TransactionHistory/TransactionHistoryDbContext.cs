using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<TransactionHistory>? TransactionsHistory { get; set; }
        public static void TransactionOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionHistory>()
                .HasKey(p => new
                {
                    p.Date,
                    p.Reference
                });
        }
    }
}