namespace IGApi.Model
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public partial class IGApiDbContext : DbContext
    {
        //TODO:     PRIOLOW_connectionString
        private static readonly string _connectionString = "Password=96ci^w4XSCP&iy&m;Database=TDBT;User ID=TheDaveTrader;Data Source=LITTLEKICK;";
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => base.OnConfiguring(
                optionsBuilder
                    .UseSqlServer(
                        _connectionString,
                        builder =>
                        {
                            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            builder.CommandTimeout(600);
                        })
                    //.UseLazyLoadingProxies() Is prone to create unneccessary db request (n+1 problem)
            );

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AccountOnModelCreating(modelBuilder);
            IGRestRequestQueueOnModelCreating(modelBuilder);
            OpenPositionOnModelCreating(modelBuilder);
            TickOnModelCreating(modelBuilder);
            EpicDetailOnModelCreating(modelBuilder);
            EpicDetailSpecialInfoOnModelCreating(modelBuilder);
            EpicDetailCurrencyOnModelCreating(modelBuilder);
            EpicDetailDepositBandOnModelCreating(modelBuilder);
            EpicDetailOpeningHourOnModelCreating(modelBuilder);
            CurrencyOnModelCreating(modelBuilder);
            WorkingOrderOnModelCreating(modelBuilder);

            //  Set default varchar(4000)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {
                if (property.GetMaxLength() == null)
                    property.SetMaxLength(4000);
            }

            // Set default decimal(38,19)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(38);
                property.SetScale(19);
            }
        }
    }
}
