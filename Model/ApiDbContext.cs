using global::IGApi.Common;
using global::IGApi.RequestQueue;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IGApi.Model
{

    using static Log;

    public partial class ApiDbContext : DbContext
    {
        //TODO:     PRIOLOW_connectionString
        public const string DbName = "TDBT";

        private static readonly string _connectionString = string.Concat("Password=4oNbM5bmDWDk2c;Database=", DbName, ";User ID=igapi;Data Source=LITTLEBIGKICK;Pooling=TRUE;TrustServerCertificate=True");
        public string ConnectionString;

        public ApiDbContext() : base()
        {
            _ = this.ApiRequestQueueItems ?? throw new DBContextNullReferenceException(nameof(this.ApiRequestQueueItems));
            _ = this.TransactionsHistory ?? throw new DBContextNullReferenceException(nameof(this.TransactionsHistory));
            _ = this.OpenPositions ?? throw new DBContextNullReferenceException(nameof(this.OpenPositions));
            _ = this.WatchlistEpicDetails ?? throw new DBContextNullReferenceException(nameof(this.WatchlistEpicDetails));
            _ = this.Watchlists ?? throw new DBContextNullReferenceException(nameof(this.Watchlists));
            _ = this.ClientSentiments ?? throw new DBContextNullReferenceException(nameof(this.ClientSentiments));
            _ = this.ActivitiesHistory ?? throw new DBContextNullReferenceException(nameof(this.ActivitiesHistory));
            _ = this.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(this.WorkingOrders));
            _ = this.Accounts ?? throw new DBContextNullReferenceException(nameof(this.Accounts));
            _ = this.SearchResults ?? throw new DBContextNullReferenceException(nameof(this.SearchResults));
            _ = this.Currencies ?? throw new DBContextNullReferenceException(nameof(this.Currencies));
            _ = this.EpicDetails ?? throw new DBContextNullReferenceException(nameof(this.EpicDetails));
            _ = this.EpicTicks ?? throw new DBContextNullReferenceException(nameof(this.EpicTicks));
            _ = this.EpicDetailsMarginDepositBand ?? throw new DBContextNullReferenceException(nameof(this.EpicDetailsMarginDepositBand));
            _ = this.EpicDetailsCurrency ?? throw new DBContextNullReferenceException(nameof(this.EpicDetailsCurrency));
            _ = this.ApiEngineState ?? throw new DBContextNullReferenceException(nameof(this.ApiEngineState));
            _ = this.ConfirmResponses ?? throw new DBContextNullReferenceException(nameof(this.ConfirmResponses));
            _ = this.MarketNodes ?? throw new DBContextNullReferenceException(nameof(this.MarketNodes));
            _ = this.EpicDetailsOpeningHour ?? throw new DBContextNullReferenceException(nameof(this.EpicDetailsOpeningHour));
            _ = this.EpicDetailsSpecialInfo ?? throw new DBContextNullReferenceException(nameof(this.EpicDetailsSpecialInfo));
            //_ = this.EpicDetailSnapshot ?? throw new DBContextNullReferenceException(nameof(this.EpicDetailSnapshot));
            _ = this.ApiEventHandlers ?? throw new DBContextNullReferenceException(nameof(this.ApiEventHandlers));
            _ = this.BaseCurrencyConvertRatios ?? throw new DBContextNullReferenceException(nameof(this.BaseCurrencyConvertRatios));
            _ = this.EpicSnapshots ?? throw new DBContextNullReferenceException(nameof(this.EpicSnapshots));
            _ = this.EpicSnapshotAllowances ?? throw new DBContextNullReferenceException(nameof(this.EpicSnapshotAllowances));

            ConnectionString = this.Database.GetConnectionString() ?? throw new DBContextConnectionStringNullException();
        }

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
            ApiEngineStatusOnModelCreating(modelBuilder);
            AccountOnModelCreating(modelBuilder);
            IGRestRequestQueueOnModelCreating(modelBuilder);
            OpenPositionOnModelCreating(modelBuilder);
            EpicTickOnModelCreating(modelBuilder);
            EpicDetailOnModelCreating(modelBuilder);
            EpicDetailSpecialInfoOnModelCreating(modelBuilder);
            //EpicDetailSnapshotOnModelCreating(modelBuilder);
            EpicDetailCurrencyOnModelCreating(modelBuilder);
            EpicDetailDepositBandOnModelCreating(modelBuilder);
            EpicDetailOpeningHourOnModelCreating(modelBuilder);
            CurrencyOnModelCreating(modelBuilder);
            WorkingOrderOnModelCreating(modelBuilder);
            ActivityOnModelCreating(modelBuilder);
            TransactionOnModelCreating(modelBuilder);
            ClientSentimentOnModelCreating(modelBuilder);
            ConfirmResponseOnModelCreating(modelBuilder);
            WatchlistOnModelCreating(modelBuilder);
            WatchlistEpicDetailOnModelCreating(modelBuilder);
            SearchResultOnModelCreating(modelBuilder);
            MarketNodeOnModelCreating(modelBuilder);
            ApiEventHandlerOnModelCreating(modelBuilder);
            BaseCurrencyConvertRatioOnModelCreating(modelBuilder);
            EpicSnapshotOnModelCreating(modelBuilder);
            EpicSnapshotAllowanceOnModelCreating(modelBuilder);

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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var isSaved = false;
            Task<int>? task = null;

            while (!isSaved)
            {
                try
                {
                    InvokeApiEvent();
                    task = Task.Run(() => base.SaveChangesAsync());
                    task.Wait(CancellationToken.None);
                    isSaved = true;
                }
                catch (AggregateException ex)
                {
                    foreach (var error in ex.InnerExceptions)
                    {
                        if (error is DbUpdateConcurrencyException exception)
                            CatchDbUpdateConcurrencyException(ref isSaved, ref task, exception);
                        else
                            throw error;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    CatchDbUpdateConcurrencyException(ref isSaved, ref task, ex);
                }
            }
            if (task is not null)
                return task;
            else
                return Task.FromResult(0);

            static void CatchDbUpdateConcurrencyException(ref bool isSaved, ref Task<int>? task, DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    if (databaseValues is not null)
                    {
                        isSaved = false;
                        foreach (var property in proposedValues.Properties)
                        {
                            proposedValues[property] = databaseValues[property];
                        }

                        WriteException(ex: new DbUpdateConcurrencyException($"Concurrency conflict arised for {entry.Metadata.Name}. Reverting to databasevalues. {ex.Message}"));

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        isSaved = true; // Record does not exists anymore, so assume status issaved.
                        WriteException(ex: new DbUpdateConcurrencyException($"Record does not exists anymore for entity {entry.Metadata.Name}. "));
                        task = Task.FromResult(0);
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            InvokeApiEvent();
            return base.SaveChanges();
        }

        private void InvokeApiEvent()
        {
            var entries = ChangeTracker.Entries()
                .Where(e =>
                (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted) &&
                e.Metadata.DisplayName() != nameof(ApiEngineStatus)
                );
            if (entries is not null && entries.Any())
            {
                var events =
                    from entry in entries
                    join api_event in ApiEventHandlers on entry.Metadata.DisplayName().ToLower() equals api_event.Sender.ToLower()
                    orderby api_event.Priority descending
                    select new
                    {
                        @delegate = api_event.Delegate,
                        sender = entry.Metadata.DisplayName(),
                        state = entry.State,
                        eventArgs = JsonConvert.SerializeObject(entry.CurrentValues.ToObject(), Formatting.None)
                    };

                if (events.Any())
                {
                    foreach (var @event in events.ToArray())    //use ToArray to avoid System.AggregateException: One or more errors occurred. (Collection was modified; enumeration operation may not execute.)
                    {
                        new ApiRequestQueueItem(
                            restRequest: nameof(RequestQueueEngineItem.ExecuteSqlScript),
                            parameters: $"exec {@event.@delegate} @state='{@event.state}', @event_arguments='{@event.eventArgs}'",
                            executeAsap: true,
                            isRecurrent: false,
                            guid: Guid.NewGuid(),
                            parentGuid: null)
                            .SaveApiRequestQueueItem(this.ConnectionString);
                    };
                }
            }
        }
    }
}
