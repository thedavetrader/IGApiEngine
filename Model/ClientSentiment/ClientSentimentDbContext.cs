using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<ClientSentiment>? ClientSentiments { get; set; }
        public static void ClientSentimentOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientSentiment>()
                .HasKey(p => new
                {
                    p.MarketId
                });
        }
    }
}
