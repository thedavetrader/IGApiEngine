using Microsoft.EntityFrameworkCore;

namespace
IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<ApiEventHandler> ApiEventHandlers { get; set; }
        public static void ApiEventHandlerOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiEventHandler>().Property(p => p.Priority).HasDefaultValue(0);

            modelBuilder.Entity<ApiEventHandler>()
                .HasKey(p => new
                {
                    p.Sender,
                    p.Delegate
                });

            modelBuilder.Entity<ApiEventHandler>(e => e.HasCheckConstraint("sender", "sender in (" +
                String.Join(",",
                    string.Format("'{0}'", nameof(Account)),
                    string.Format("'{0}'", nameof(ActivityHistory)),
                    string.Format("'{0}'", nameof(ApiEngineStatus)),
                    string.Format("'{0}'", nameof(ApiEventHandler)),
                    string.Format("'{0}'", nameof(ApiRequestQueueItem)),
                    string.Format("'{0}'", nameof(ClientSentiment)),
                    string.Format("'{0}'", nameof(ConfirmResponse)),
                    string.Format("'{0}'", nameof(Currency)),
                    string.Format("'{0}'", nameof(EpicDetail)),
                    string.Format("'{0}'", nameof(EpicDetailCurrency)),
                    string.Format("'{0}'", nameof(EpicDetailMarginDepositBand)),
                    string.Format("'{0}'", nameof(EpicDetailOpeningHour)),
                    string.Format("'{0}'", nameof(EpicDetailSpecialInfo)),
                    string.Format("'{0}'", nameof(EpicTick)),
                    string.Format("'{0}'", nameof(MarketNode)),
                    string.Format("'{0}'", nameof(OpenPosition)),
                    string.Format("'{0}'", nameof(SearchResult)),
                    string.Format("'{0}'", nameof(TransactionHistory)),
                    string.Format("'{0}'", nameof(Watchlist)),
                    string.Format("'{0}'", nameof(WatchlistEpicDetail)),
                    string.Format("'{0}'", nameof(WorkingOrder))
                ) + ")"));
        }
    }
}
