using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<BaseCurrencyConvertRatio> BaseCurrencyConvertRatios { get; set; }
        public static void BaseCurrencyConvertRatioOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseCurrencyConvertRatio>()
                .ToView(nameof(Base64FormattingOptions))
                .HasKey(p => p.Epic);
        }
    }
}