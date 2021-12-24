
go

if type_id('epic_list') is null
    create type epic_list as table (
        epic   nvarchar(128)    primary key nonclustered )
    with (memory_optimized = on)

go


create or alter procedure create_watchlist
    @name       nvarchar(4000)
,   @epic_list  epic_list readonly
as begin

    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap    )
    select  request         = 'CreateWatchlist'
    ,       parameter       =   (   select  name    = @name
                                    ,       epics   = json_query( '['   
                                                                + ( select  string_agg (quotename(ed.epic, '"'), ',')
                                                                    from    @epic_list  el
                                                                    join    epic_detail ed
                                                                    on      ed.epic     = el.epic )
                                                                + ']')
                                    for json path, without_array_wrapper  )
    ,       execute_asap    = 1

    --if @@rowcount = 0
    --    throw 51000, 'The specified watchlistid or epic could not be found', 1

end

go

--declare @epic_list epic_list

--insert  @epic_list select epic from epic_detail

--exec    create_watchlist 
--        @name       = 'TEST'
--,       @epic_list  = @epic_list

/*

{"name":"TEST","epics":["CS.D.BITCOIN.CFD.IP","CS.D.ETHUSD.CFE.IP","IX.D.RUSSELL.IFM.IP","IX.D.SPTRD.IFE.IP","UC.D.PYPLVUS.CASH.IP"]}
{"name":"TESTTT","epics":["CS.D.BITCOIN.CFD.IP","CS.D.ETHUSD.CFE.IP"]}
*/