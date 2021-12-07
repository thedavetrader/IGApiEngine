using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<EpicDetail>? EpicDetails { get; set; }
        public static void EpicDetailOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicDetail>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetail>()
                .HasKey(p => new
                {
                    p.Epic
                });



            //modelBuilder.Entity<EpicDetail>()
            //    .HasMany<EpicDetailSpecialInfo>(p => p.SpecialInfo)
            //    .WithOne(p => p.EpicDetail);

            //modelBuilder.Entity<EpicDetail>()
            //    .HasMany<EpicDetailCurrency>(p => p.Currency)
            //    .WithOne(p => p.EpicDetail);

            //modelBuilder.Entity<EpicDetail>()
            //    .HasMany<EpicDetailMarginDepositBand>(p => p.MarginDepositBand)
            //    .WithOne(p => p.EpicDetail);
        }
    }
}