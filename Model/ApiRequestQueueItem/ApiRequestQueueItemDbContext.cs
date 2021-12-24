using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
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

            modelBuilder.Entity<ApiRequestQueueItem>().HasAlternateKey(k => k.Guid);

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Guid).HasDefaultValueSql("newid()");

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Request).HasMaxLength(512);

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Parameters).HasColumnType("nvarchar(max)");

            modelBuilder.Entity<ApiRequestQueueItem>(e => e.HasCheckConstraint("request", "request in (" +
                String.Join(",",
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetAccountDetails)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetOpenPositions)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetWorkingOrders)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetEpicDetails)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetActivityHistory)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetTransactionHistory)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetClientSentiment)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.CreatePosition)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.EditPosition)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.ClosePosition)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.CreateWorkingOrder)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.EditWorkingOrder)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.DeleteWorkingOrder)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetWatchlists)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.CreateWatchlist)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.DeleteWatchlist)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.GetWatchListEpics)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.AddWatchlistEpic)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.RemoveWatchlistEpic)),
                string.Format("'{0}'", nameof(RequestQueue.RequestQueueEngineItem.Search))
                ) + ")"));

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}