using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<WatchlistEpicDetail> WatchlistEpicDetails { get; set; }
        public static void WatchlistEpicDetailOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WatchlistEpicDetail>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<WatchlistEpicDetail>()
                 .HasKey(k => new { k.AccountId, k.WatchlistId, k.Epic });

            modelBuilder.Entity<WatchlistEpicDetail>()
                .HasOne(w => w.Watchlist)
                .WithMany(w => w.WatchlistEpicDetails)
                .HasForeignKey(w => new { w.AccountId, w.WatchlistId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchlistEpicDetail>()
                .HasOne(e => e.EpicDetail)
                .WithMany(e => e.WatchlistEpicDetails)
                .HasForeignKey(e => e.Epic)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}