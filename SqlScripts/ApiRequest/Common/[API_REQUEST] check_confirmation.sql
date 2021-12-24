
go

create or alter procedure check_confirmation
        @deal_reference             nvarchar(36)    = null
,       @deal_id                    nvarchar(64)    = null
,       @epic                       nvarchar(4000)  = null
as begin

    if @deal_reference is null and @deal_id is null and @epic is null
        throw 51000, 'Set at least one of "@deal_reference", "@deal_id" or "@epic".', 1

    if      iif(@deal_reference is not null, 1, 0)
        +   iif(@deal_id        is not null, 1, 0) 
        +   iif(@epic           is not null, 1, 0) > 1 
        throw 51000, 'Set at least no more then one of "@deal_reference", "@deal_id" or "@epic".', 1

    /*  
        Wait for confirmation
    */
    declare @message                    nvarchar(max)
    declare @timeout                    int             = 4 * 100;
    declare @found                      bit = 0
    declare @api_engine_status_is_alive bit = 0
    declare @retry                      int = 0
    declare @deal_status                nvarchar(4000)
    declare @response                   nvarchar(max)
    
    while   (   @found = 0  or @api_engine_status_is_alive = 0)
    and     @retry < @timeout
    begin

        set     @api_engine_status_is_alive = dbo.get_api_engine_status_is_alive()

        select  @found              = 1
        ,       @deal_status        = cr.deal_status
        ,       @response           =   (   select  *
                                            from    confirm_response j
                                            where   j.deal_reference    = cr.deal_reference
                                            for JSON path, Without_Array_Wrapper    )
        from    confirm_response    cr
        where   (   cr.deal_id          = @deal_id
                or  cr.epic             = @epic 
                or  cr.deal_reference   = @deal_reference   )

        waitfor delay '00:00:00:010'

        set @retry += 1

    end

    if @found = 1 and @api_engine_status_is_alive = 1
    begin

        delete  cr
        from    confirm_response    cr
        where   cr.deal_reference   = @deal_reference 

        set @message = 'Confirmation received for OTC request:'
        
        raiserror(@message , 10, 10) with nowait

        set @response = dbo.indent_json_object(@response)
        raiserror(@response, 10, 10) with nowait

        if @deal_status = 'ACCEPTED'
            return 0
        else
            return -1

    end
    else
    begin
        
        if  @api_engine_status_is_alive = 0
        begin 

            --      Api interface is not alive or is in abnormal state. 
            --      Prevent the request will be untimely executed when interface is recovered by removing it from queue.
            delete  rrq
            from    api_request_queue_item  rrq
            where   json_value(rrq.parameter, '$.dealReference') = @deal_reference

            set @message = 'ApiEngine does not report active and alive.'
            raiserror(@message, 10, 10) with nowait

        end
        
        if  @found = 0
        begin
        
            set @message = 'Failed to receive confirmation for OTC request within reasonable time. Dealreference "' + @deal_reference + '".'
            raiserror(@message, 10, 10) with nowait

        end

        return -2

    end
end

go
