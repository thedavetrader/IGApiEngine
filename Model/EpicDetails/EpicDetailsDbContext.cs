using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<EpicDetail>? EpicDetails { get; set; }
        public DbSet<EpicDetail.EpicDetailSpecialInfo>? EpicDetailsSpecialInfo { get; set; }
        public static void EpicDetailOnModelCreating(ModelBuilder modelBuilder)
        {
            #region EpicDetail
            modelBuilder.Entity<EpicDetail>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetail>()
                .HasKey(p => new
                {
                    p.Epic
                });

            modelBuilder.Entity<EpicDetail>()
                .HasOne<EpicDetail.EpicDetailSpecialInfo>(p => p.SpecialInfo)
                .WithOne(p => p.EpicDetail)
                .HasForeignKey<EpicDetail.EpicDetailSpecialInfo>(p => p.Epic);
            #endregion

            #region EpicDetail.EpicDetailSpecialInfo
            modelBuilder.Entity<EpicDetail.EpicDetailSpecialInfo>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetail.EpicDetailSpecialInfo>()
            .HasKey(p => new
            {
                p.Epic
            });
            #endregion
        }
    }
}