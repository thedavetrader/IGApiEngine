using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
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
        }
    }
}