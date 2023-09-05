using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<Watchlist> Watchlists { get; set; }
        public static void WatchlistOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Watchlist>().Property(p => p.WatchlistId).ValueGeneratedNever();

            modelBuilder.Entity<Watchlist>()
                .HasKey(p => new
                {
                    p.AccountId,
                    p.WatchlistId
                });
        }
    }
}