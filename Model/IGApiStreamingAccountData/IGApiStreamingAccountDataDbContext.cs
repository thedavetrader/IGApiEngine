using dto.endpoint.accountbalance;
using IGWebApiClient;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<IGApiStreamingAccountData>? IGApiStreamingAccountData { get; set; }
        public static void StreamingAccountDataOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IGApiStreamingAccountData>(e => e.ToTable("StreamingAccountData"));

            modelBuilder.Entity<IGApiStreamingAccountData>().HasKey(p => new
            {
                p.AccountId
            });
        }
    }
}
