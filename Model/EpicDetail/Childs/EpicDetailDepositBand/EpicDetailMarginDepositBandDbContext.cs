using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicDetailMarginDepositBand>? EpicDetailsMarginDepositBand { get; set; }
        public static void EpicDetailDepositBandOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicDetailMarginDepositBand>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetailMarginDepositBand>()
                .HasKey(p => new
                {
                    p.Epic,
                    p.Currency,
                    p.Min
                });

            modelBuilder.Entity<EpicDetailMarginDepositBand>()
                .HasOne(p => p.EpicDetail)
                .WithMany(b => b.MarginDepositBands);
        }
    }
}