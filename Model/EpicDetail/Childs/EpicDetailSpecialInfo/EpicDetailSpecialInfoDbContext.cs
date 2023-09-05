using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicDetailSpecialInfo> EpicDetailsSpecialInfo { get; set; }
        public static void EpicDetailSpecialInfoOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicDetailSpecialInfo>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetailSpecialInfo>()
                .HasKey(p => new
                {
                    p.Epic,
                    p.SpecialInfo
                });

            modelBuilder.Entity<EpicDetailSpecialInfo>()
                .HasOne(p => p.EpicDetail)
                .WithMany(b => b.SpecialInfo);
        }
    }
}