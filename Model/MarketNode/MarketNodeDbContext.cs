using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<MarketNode> MarketNodes { get; set; }
        public static void MarketNodeOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MarketNode>()
                .HasKey(p => new
                {
                    p.MarketNodeId
                });
        }
    }
}