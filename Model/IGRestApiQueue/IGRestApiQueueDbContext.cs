using dto.endpoint.accountbalance;
using IGWebApiClient;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<IGRestApiQueue>? IGRestApiQueue { get; set; }
        public static void IGRestApiQueueOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IGRestApiQueue>().IsMemoryOptimized(true);

            modelBuilder.Entity<IGRestApiQueue>().HasIndex(p => new
            {
                p.ExecuteAsap,
                p.Timestamp
            });

            modelBuilder.Entity<IGRestApiQueue>().Property(p => p.RestRequest).HasMaxLength(512);
            modelBuilder.Entity<IGRestApiQueue>(e => e.HasCheckConstraint("CHK_IGRestApiQueue_RestRequest", "RestRequest in (" +
                "'GetAccountDetails', " +
                "'GetOpenPositions', " +
                "'CreatePosition'" +
                ")"));

            modelBuilder.Entity<IGRestApiQueue>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<IGRestApiQueue>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<IGRestApiQueue>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}