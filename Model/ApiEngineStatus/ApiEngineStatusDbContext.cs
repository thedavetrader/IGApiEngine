using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<ApiEngineStatus> ApiEngineState { get; set; }
        public static void ApiEngineStatusOnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ApiEngineStatus>().IsMemoryOptimized();

            modelBuilder.Entity<ApiEngineStatus>()
                .HasKey(p => new
                {
                    p.Id
                });
        }
    }
}