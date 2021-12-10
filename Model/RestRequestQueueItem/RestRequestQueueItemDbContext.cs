using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<RestRequestQueueItem>? RestRequestQueue { get; set; }
        public static void IGRestRequestQueueOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RestRequestQueueItem>().IsMemoryOptimized(true);

            modelBuilder.Entity<RestRequestQueueItem>().HasIndex(p => new
            {
                p.ExecuteAsap,
                p.Timestamp
            });

            modelBuilder.Entity<RestRequestQueueItem>().Property(p => p.RestRequest).HasMaxLength(512);

            modelBuilder.Entity<RestRequestQueueItem>(e => e.HasCheckConstraint("rest_request", "rest_request in (" +
                $"'{nameof(RestRequest.RestRequest.GetAccountDetails)}'" + "," +
                $"'{nameof(RestRequest.RestRequest.GetOpenPositions)}'" + "," +
                $"'{nameof(RestRequest.RestRequest.GetWorkingOrders)}'" + "," +
                $"'{nameof(RestRequest.RestRequest.GetEpicDetails)}'" +
                ")"));

            modelBuilder.Entity<RestRequestQueueItem>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<RestRequestQueueItem>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<RestRequestQueueItem>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}