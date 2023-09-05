using IGApi.RequestQueue;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiDbContext
    {
        public DbSet<ApiRequestQueueItem> ApiRequestQueueItems { get; set; }
        public static void IGRestRequestQueueOnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ApiRequestQueueItem>().IsMemoryOptimized(true);

            modelBuilder.Entity<ApiRequestQueueItem>().HasIndex(p => new
            {
                p.ExecuteAsap,
                p.Timestamp
            });

            modelBuilder.Entity<ApiRequestQueueItem>().HasKey(k => k.Guid);

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Guid).HasDefaultValueSql("newid()");

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Request).HasMaxLength(512);

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Parameters).HasColumnType("nvarchar(max)");

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.RecurrencyInterval).HasDefaultValue(1);

            var methods = typeof(RequestQueueEngineItem)
                            .GetMethods()
                            .Select(method => new { methodName = method.Name, requestTypeAttribute = (RequestTypeAttribute?)Attribute.GetCustomAttribute(method, typeof(RequestTypeAttribute)) })
                            .Where(w => w.requestTypeAttribute is not null)
                            .Select(s => string.Format("'{0}'", s.methodName));

            modelBuilder.Entity<ApiRequestQueueItem>(e => 
                e.HasCheckConstraint("request", "request in (" + String.Join(",", methods) + ")"));

            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.Timestamp).HasDefaultValueSql("getutcdate()");
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.ExecuteAsap).HasDefaultValue(false);
            modelBuilder.Entity<ApiRequestQueueItem>().Property(p => p.IsRecurrent).HasDefaultValue(false);
        }
    }
}