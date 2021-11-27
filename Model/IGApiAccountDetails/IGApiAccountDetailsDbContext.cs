using dto.endpoint.accountbalance;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<IGApiAccountDetails>? IGApiAccountDetails { get; set; }
        public static void AccountDetailsOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IGApiAccountDetails>(e => e.ToTable("AccountDetails"));

            modelBuilder.Entity<IGApiAccountDetails>()
                .Ignore(p => p.balance);

            modelBuilder.Entity<IGApiAccountDetails>()
                .HasKey(p => new
                {
                    p.accountId
                });
        }
    }
}
