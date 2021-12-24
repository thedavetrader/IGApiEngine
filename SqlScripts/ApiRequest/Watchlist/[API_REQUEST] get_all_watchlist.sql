
go

create or alter procedure get_all_watchlist
as begin

    insert  api_request_queue_item (
            request
    ,       execute_asap    )
    select  request         = 'GetWatchlists'
    ,       execute_asap    = 1

end

go

--exec    get_all_watchlist 
