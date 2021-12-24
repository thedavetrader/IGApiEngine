
go

create or alter procedure get_watchlist_epic
    @watchlist_id nvarchar(4000)
as begin

    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap )
    select  request       = 'GetWatchListEpics'
    ,       parameter     = w.watchlist_id
    ,       execute_asap  = 1
    from    watchlist     w
    where   w.watchlist_id  = @watchlist_id 

    if @@rowcount = 0
        throw 51000, 'The specified watchlistid could not be found', 1

end

go
