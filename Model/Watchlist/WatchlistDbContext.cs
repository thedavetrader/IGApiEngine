using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<Watchlist>? Watchlists { get; set; }
        public static void WatchlistOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Watchlist>().Property(p => p.Id).ValueGeneratedNever();

            modelBuilder.Entity<Watchlist>()
                .HasKey(p => new
                {
                    p.AccountId,
                    p.Id
                });
        }
    }
}