using dto.endpoint.accountbalance;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public static void AccountOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasKey(p => new
                {
                    p.AccountId
                });
        }
    }
}
