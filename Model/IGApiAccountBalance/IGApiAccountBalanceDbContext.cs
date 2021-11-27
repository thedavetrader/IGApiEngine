using dto.endpoint.accountbalance;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<IGApiAccountBalance>? AccountBalances { get; set; }

        public static void AccountBalanceOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IGApiAccountBalance>(e => e.ToTable("AccountBalance"));

            modelBuilder.Entity<IGApiAccountBalance>()
                .HasKey(p => new
                {
                    p.AccountId
                });
        }
    }
}
