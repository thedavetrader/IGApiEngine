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

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Request).HasMaxLength(512);

            modelBuilder.Entity<ApiRequestQueueItem>(e => e.HasCheckConstraint("request", "request in (" +
                String.Join(",",
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetAccountDetails)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetOpenPositions)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetWorkingOrders)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetActivityHistory)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetTransactionHistory)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetClientSentiment)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.CreatePosition)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.EditPosition)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.ClosePosition)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.CreateWorkingOrder)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.EditWorkingOrder)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.DeleteWorkingOrder)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetWatchlists)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.CreateWatchlist)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.DeleteWatchlist)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueItem.GetEpicDetails))
                ) + ")"));

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}