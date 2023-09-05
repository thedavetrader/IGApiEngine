using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public static void CurrencyOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasKey(p => new
                {
                    p.Code
                });
        }
    }
}