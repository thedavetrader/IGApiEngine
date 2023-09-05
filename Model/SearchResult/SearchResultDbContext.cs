using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<SearchResult> SearchResults { get; set; }
        public static void SearchResultOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchResult>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<SearchResult>()
                 .HasKey(k => new { k.Epic });
        }
    }
}