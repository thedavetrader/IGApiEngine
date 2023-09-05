using System.Data;
using System.Data.SqlClient;
using IGApi.Common;
using IGApi.Model;
using Microsoft.EntityFrameworkCore;

namespace IGApi.RequestQueue
{
    using static Log;

    internal static partial class RequestQueueEngine
    {
        public static bool IsInitialized
        {
            get
            {
                return
                    _getAccountDetailsCompleted &&
                    _getOpenPositionsCompleted &&
                    _getWorkingOrdersCompleted &&
                    _getWatchlistsCompleted;
            }
        }

        /// <summary>
        /// Make sure that the database has all the neccessary objects that the ApiEngine relies on.
        /// The sqlscripts are deliberately not stored in separate sql files, because that would
        /// require these files to be shipped with the endproduct. Furthermore, it would also impose 
        /// a security risk, because those files could easily be modified.
        /// 
        /// The method for executing sql scripts is called directly, the db queue can not be used in 
        /// this stage of the inialization. Acces to the db queue relies on the db objects.
        /// </summary>
        private static void InitDbObjects()
        {
            List<string> sql = new()
            {
                @"
                    alter database [" + ApiDbContext.DbName + @"] modify file(name = N'tdbt', filegrowth = 1048576kb);
                    alter database [" + ApiDbContext.DbName + @"] modify file(name = N'tdbt_log', filegrowth = 1048576kb);
                    alter database [" + ApiDbContext.DbName + @"] set recovery simple with no_wait;
                ",

                @"
                if type_id(N'request_list') is null
                create type request_list as table (
                    request             nvarchar(64) primary key nonclustered hash with(bucket_count = 131072)
                ,   is_rest_request     bit
                ,   is_trading_request  bit
                ) with(memory_optimized = on)
                ",

                @"
                create or alter procedure dbo.pop_api_request_queue_item
                    @request_list dbo.request_list  readonly
                ,   @on_cycle     bit
                as begin

                  declare @guid         uniqueidentifier
                  declare @parent_guid  uniqueidentifier
                  declare @timestamp    datetime2
                  declare @request      nvarchar(512)   
                  declare @parameter    nvarchar(max)
                  declare @execute_asap bit
                  declare @is_recurrent bit

                  select top 1
                          @guid         = arqi.guid
                  ,       @parent_guid  = arqi.parent_guid
                  ,       @timestamp    = arqi.timestamp
                  ,       @request      = arqi.request
                  ,       @parameter    = arqi.parameter
                  ,       @execute_asap = arqi.execute_asap
                  ,       @is_recurrent = arqi.is_recurrent
                  from    dbo.api_request_queue_item arqi
                  join    @request_list   el
                  on      el.request      = arqi.request
                  where   arqi.timestamp  <= getutcdate()
                  and     (   @on_cycle = 1 and el.is_trading_request = 0
                          or  @on_cycle = 0 and (     el.is_rest_request = 0 and el.is_trading_request = 0
                                                  or  el.is_trading_request = 1) )
                  order
                  by      el.is_trading_request               desc
                  ,       arqi.execute_asap                   desc
                  ,       iif(arqi.parent_guid is null, 0, 1) desc
                  ,       arqi.timestamp                      asc

                  update  dbo.api_request_queue_item
                  set     timestamp     = dateadd(second, recurrency_interval, getutcdate())
                  where   guid          = @guid
                  and     is_recurrent  = 1

                  delete
                  from    dbo.api_request_queue_item
                  where   guid         = @guid
                  and     is_recurrent = 0

                  select guid         = @guid
                  ,      parent_guid  = @parent_guid
                  ,      timestamp    = @timestamp
                  ,      request      = @request
                  ,      parameter    = @parameter
                  ,      execute_asap = @execute_asap
                  ,      is_recurrent = @is_recurrent

                end            
                ",

                @"
                create or alter procedure dbo.push_api_request_queue_item
                    @guid           uniqueidentifier
                ,   @parent_guid    uniqueidentifier = null
                ,   @timestamp      datetime2
                ,   @request        nvarchar(512)   
                ,   @parameter      nvarchar(max)   = null
                ,   @execute_asap   bit
                , @is_recurrent     bit
                as begin
                if @guid is not null
                    insert dbo.api_request_queue_item
                    (   guid
                    ,   parent_guid
                    ,   timestamp
                    ,   request
                    ,   parameter
                    ,   execute_asap
                    ,   is_recurrent)
                    select @guid
                    ,      @parent_guid
                    ,      @timestamp
                    ,      @request
                    ,      @parameter
                    ,      @execute_asap
                    ,      @is_recurrent
                end
                ",

                @"
                drop procedure if exists dbo.upsert_epic_tick
                ",

                @"
                create or alter procedure dbo.upsert_epic_tick
                    @epic               nvarchar(128)
                ,   @mid_open           decimal(38,19)   = null
                ,   @high               decimal(38,19)   = null
                ,   @low                decimal(38,19)   = null
                ,   @change             decimal(38,19)   = null
                ,   @change_percentage  decimal(38,19)   = null
                ,   @update_time        datetime2(7)
                ,   @market_delay       int              = null
                ,   @market_state       nvarchar(4000)
                ,   @bid                decimal(38,19)   = null
                ,   @offer              decimal(38,19)   = null 
                ,   @result             nvarchar(max)   output
                with native_compilation, schemabinding, execute as owner as begin atomic with (transaction isolation level = snapshot, language = N'English')

                  update  dbo.epic_tick  
                  set     mid_open          = @mid_open         
                  ,       high              = @high             
                  ,       low               = @low              
                  ,       change            = @change           
                  ,       change_percentage = @change_percentage
                  ,       update_time       = @update_time      
                  ,       market_delay      = @market_delay     
                  ,       market_state      = @market_state     
                  ,       bid               = @bid              
                  ,       offer             = @offer            
                  where   epic = @epic

                  if @@rowcount = 0
                  begin

                    insert  dbo.epic_tick (
                            epic
                    ,       mid_open          
                    ,       high              
                    ,       low               
                    ,       change            
                    ,       change_percentage 
                    ,       update_time       
                    ,       market_delay      
                    ,       market_state      
                    ,       bid               
                    ,       offer )
                    select  @epic
                    ,       @mid_open         
                    ,       @high             
                    ,       @low              
                    ,       @change           
                    ,       @change_percentage
                    ,       @update_time      
                    ,       @market_delay     
                    ,       @market_state     
                    ,       @bid              
                    ,       @offer            

                    set @result = 'Added'

                  end
                  else
                    set @result = 'Modified'

                end
                "
            };

            bool @continue = false;

            void ExecuteSqlScriptCompleted(object? sender, EventArgs e) { @continue = true; }
            RequestQueueEngineItem.ExecuteSqlScriptCompleted += ExecuteSqlScriptCompleted;

            sql.ForEach(sqlScript =>
            {
                @continue = false;

                ApiRequestQueueItem apiRequestQueueItem = new(
                    restRequest: nameof(RequestQueueEngineItem.ExecuteSqlScript),
                    parameters: sqlScript,
                    executeAsap: true,
                    isRecurrent: false,
                    guid: Guid.NewGuid(),
                    parentGuid: null);

                new RequestQueueEngineItem(apiRequestQueueItem, DateTime.UtcNow, _cancellationToken).Execute();

                while (!@continue) { /* Wait for sql script to complete to prevent deadlock. */ }
            });

            RequestQueueEngineItem.ExecuteSqlScriptCompleted -= ExecuteSqlScriptCompleted;

            WriteOk(Columns("Initialization done", nameof(InitDbObjects)));
        }

        /// <summary>
        /// Open positions and working orders could be changed during the time the API was not running. 
        /// Therefor assume the current state invalid and regard them as obsolete and to be removed.
        /// </summary>
        /// <exception cref="DBContextNullReferenceException"></exception>
        private static void InitTables()
        {
            using ApiDbContext apiDbContext = new();

            //  Clear volatile tables
            apiDbContext.ApiRequestQueueItems.RemoveRange(
                apiDbContext.ApiRequestQueueItems
                    .ToList()
                    .Where(r => ExposedRequests
                        .Where(w => w.RequestTypeAttribute != null && w.RequestTypeAttribute.IsRestRequest)
                        .Select(s => s.Request).Contains(r.Request)));
            apiDbContext.SaveChanges(); // Only remove restrequests.

            apiDbContext.ConfirmResponses.RemoveRange(apiDbContext.ConfirmResponses); apiDbContext.SaveChanges();
            apiDbContext.ApiEventHandlers.RemoveRange(apiDbContext.ApiEventHandlers); apiDbContext.SaveChanges();
            //TODO: Future: do not empty and store epicticks for all epic details (on get)
            //apiDbContext.EpicTicks.RemoveRange(apiDbContext.EpicTicks); apiDbContext.SaveChanges();

            // Bootstrap tables
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.ExecuteSqlScript), executeAsap: true, isRecurrent: false, guid: Guid.NewGuid(), parentGuid: null, parameters: "exec bootstrap", cancellationToken: _cancellationToken);

            WriteOk(Columns("Initialization done", nameof(InitTables)));
        }

        /// <summary>
        /// Make sure essential details are queued to recurrently refresh.
        /// </summary>
        private static void InitApiRequestQueueItems()
        {
            static void GetAccountDetailsCompleted(object? sender, EventArgs e) { _getAccountDetailsCompleted = true; RequestQueueEngineItem.GetAccountDetailsCompleted -= GetAccountDetailsCompleted; }
            static void GetOpenPositionsCompleted(object? sender, EventArgs e) { _getOpenPositionsCompleted = true; RequestQueueEngineItem.GetOpenPositionsCompleted -= GetOpenPositionsCompleted; }
            static void GetWorkingOrdersCompleted(object? sender, EventArgs e) { _getWorkingOrdersCompleted = true; RequestQueueEngineItem.GetWorkingOrdersCompleted -= GetWorkingOrdersCompleted; }
            static void GetWatchlistsCompleted(object? sender, EventArgs e) { _getWatchlistsCompleted = true; RequestQueueEngineItem.GetWatchlistsCompleted -= GetWatchlistsCompleted; }

            RequestQueueEngineItem.GetAccountDetailsCompleted += GetAccountDetailsCompleted;
            RequestQueueEngineItem.GetOpenPositionsCompleted += GetOpenPositionsCompleted;
            RequestQueueEngineItem.GetWorkingOrdersCompleted += GetWorkingOrdersCompleted;
            RequestQueueEngineItem.GetWatchlistsCompleted += GetWatchlistsCompleted;

            /*  System critical entities  */
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetAccountDetails), false, true, Guid.NewGuid(), cancellationToken: _cancellationToken);
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetOpenPositions), false, true, Guid.NewGuid(), cancellationToken: _cancellationToken);
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetWorkingOrders), false, true, Guid.NewGuid(), cancellationToken: _cancellationToken);
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetWatchlists), false, false, Guid.NewGuid(), cancellationToken: _cancellationToken);   // Init only, not recurrent. One-way, api point of view. Only update watchlists when changes are initiated by api.

            /*  System non-critical entities  */
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetActivityHistory), false, true, Guid.NewGuid(), cancellationToken: _cancellationToken);
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetTransactionHistory), false, true, Guid.NewGuid(), cancellationToken: _cancellationToken);
            RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.BrowseEpics), false, true, Guid.NewGuid(), cancellationToken: _cancellationToken);

            WriteOk(Columns("Initialization done", nameof(InitApiRequestQueueItems)));
        }

        public static void ResetInitialization()
        {
            _getAccountDetailsCompleted = false;
            _getOpenPositionsCompleted = false;
            _getWorkingOrdersCompleted = false;
            _getWatchlistsCompleted = false;

            InitTables();
        }
    }
}