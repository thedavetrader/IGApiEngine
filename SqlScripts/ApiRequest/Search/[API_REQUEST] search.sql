
go

create or alter procedure search
    @search_term nvarchar(4000)
as begin

    if dbo.get_api_engine_status_is_alive() = 0
        throw 51000, 'Api enginen and/or requestqueuengine is not running or fully initialized.', 1

    declare @wait_time_out  int = 500

    delete 
    from    search_result

    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap    )
    select  request         = 'Search'
    ,       parameter       = @search_term
    ,       execute_asap    = 1

    while   not exists ( select 1 from search_result  )
    and     @wait_time_out  > 0
    begin
        waitfor delay '00:00:00:100' -- waiting for search_result
        set @wait_time_out -= 1
    end

    if @wait_time_out > 1
    begin
        if exists (select 1 from search_result where epic = '[NO_RESULTS_FOUND]')
            select  '[NO_RESULTS_FOUND]'
        else
            select  ed.*
            from    search_result   sr
            join    epic_detail     ed
            on      ed.epic         = sr.epic
    end
    else
        throw 51000, 'Search returned no response within reasonable time.', 1

end

go

--delete  ed
--from    epic_detail ed
--where   exists ( select 1 from search_result sr where sr.epic = ed.epic)

--exec    search 'indices'
