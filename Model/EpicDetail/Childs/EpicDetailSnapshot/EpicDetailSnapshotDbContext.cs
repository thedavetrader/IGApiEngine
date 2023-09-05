using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<EpicDetailSnapshot> EpicDetailSnapshot { get; set; }
        public static void EpicDetailSnapshotOnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<EpicDetailSnapshot>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetailSnapshot>()
                .HasKey(p => new
                {
                    p.Epic
                });

            //modelBuilder.Entity<EpicDetailSnapshot>()
            //    .HasOne(e => e.EpicDetail)
            //    .WithOne(e => e.Snapshot)
            //    //.HasForeignKey(e => e.Epic)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}