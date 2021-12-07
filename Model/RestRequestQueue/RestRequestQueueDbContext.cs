using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<RestRequestQueue>? RestRequestQueue { get; set; }
        public static void IGRestRequestQueueOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RestRequestQueue>().IsMemoryOptimized(true);

            modelBuilder.Entity<RestRequestQueue>().HasIndex(p => new
            {
                p.ExecuteAsap,
                p.Timestamp
            });

            modelBuilder.Entity<RestRequestQueue>().Property(p => p.RestRequest).HasMaxLength(512);
            modelBuilder.Entity<RestRequestQueue>(e => e.HasCheckConstraint("rest_request", "rest_request in (" +
                $"'{nameof(RestRequest.RestRequest.GetAccountDetails)}', " +
                $"'{nameof(RestRequest.RestRequest.GetOpenPositions)}', " +
                $"'CreatePosition'," +
                $"'{nameof(RestRequest.RestRequest.GetEpicDetails)}'" +
                ")"));

            modelBuilder.Entity<RestRequestQueue>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<RestRequestQueue>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<RestRequestQueue>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}