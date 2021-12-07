using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class IGApiDbContext
    {
        public DbSet<EpicDetailOpeningHour>? EpicDetailsOpeningHour { get; set; }
        public static void EpicDetailOpeningHourOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpicDetailOpeningHour>().Property(p => p.Epic).HasMaxLength(128);

            modelBuilder.Entity<EpicDetailOpeningHour>()
                .HasKey(p => new
                {
                    p.Epic,
                    p.OpenTime
                });

            modelBuilder.Entity<EpicDetailOpeningHour>()
                .HasOne(p => p.EpicDetail)
                .WithMany(b => b.OpeningHours);
        }
    }
}