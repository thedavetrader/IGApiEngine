using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<EpicDetailCurrency>? EpicDetailsCurrency { get; set; }
        public static void EpicDetailCurrencyOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicDetailCurrency>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetailCurrency>()
                .HasKey(p => new
                {
                    p.Epic,
                    p.Code
                });

            modelBuilder.Entity<EpicDetailCurrency>()
                .HasOne(p => p.EpicDetail)
                .WithMany(b => b.Currencies);

            modelBuilder.Entity<EpicDetailCurrency>()
                .HasOne(p => p.Currency)
                .WithMany(b => b.Currencies)
                .OnDelete(DeleteBehavior.Restrict); // Reference entities should can not be deleted while other entities still refer to it.
        }
    }
}