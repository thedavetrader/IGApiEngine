
go

create or alter procedure delete_watchlist
    @watchlist_id   nvarchar(4000)
as begin

    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap    )
    select  request         = 'DeleteWatchlist'
    ,       parameter       = wl.watchlist_id
    ,       execute_asap    = 1
    from    watchlist       wl
    where   wl.watchlist_id = @watchlist_id

    if @@rowcount = 0
        throw 51000, 'The specified watchlistid or epic could not be found', 1

end

go

--exec    delete_watchlist 
--        @watchlist_id   = 16506649
