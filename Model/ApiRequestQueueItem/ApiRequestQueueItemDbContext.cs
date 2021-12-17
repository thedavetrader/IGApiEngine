using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<ApiRequestQueueItem>? ApiRequestQueueItems { get; set; }
        public static void IGRestRequestQueueOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiRequestQueueItem>().IsMemoryOptimized(true);

            modelBuilder.Entity<ApiRequestQueueItem>().HasIndex(p => new
            {
                p.ExecuteAsap,
                p.Timestamp
            });

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.RestRequest).HasMaxLength(512);

            modelBuilder.Entity<ApiRequestQueueItem>(e => e.HasCheckConstraint("rest_request", "rest_request in (" +
                $"'{nameof(RestRequest.ApiRequestItem.GetAccountDetails)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.GetOpenPositions)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.GetWorkingOrders)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.GetActivityHistory)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.GetTransactionHistory)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.GetClientSentiment)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.CreatePosition)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.EditPosition)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.ClosePosition)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.CreateWorkingOrder)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.EditWorkingOrder)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.DeleteWorkingOrder)}'" + "," +
                $"'{nameof(RestRequest.ApiRequestItem.GetEpicDetails)}'" +
                ")"));

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}