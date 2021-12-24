
go

create or alter procedure remove_watchlist_epic
    @watchlist_id nvarchar(4000)
,   @epic         nvarchar(128)
as begin

    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap    )
    select  request         = 'RemoveWatchlistEpic'
    ,       parameter       = ( select  watchlistId = w.watchlist_id
                                ,       epic        = ed.epic
                                for JSON path, Without_Array_Wrapper  )
    ,       execute_asap    = 1
    from    watchlist       w
    cross 
    join    epic_detail     ed
    where   ed.epic         = @epic         
    and     w.watchlist_id  = @watchlist_id 

    if @@rowcount = 0
        throw 51000, 'The specified watchlistid or epic could not be found', 1

end

go
