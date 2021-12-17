create or alter procedure check_confirmation
    @deal_reference nvarchar(36)
as begin

    set nocount on

    /*  
        Wait for confirmation
    */
    declare @message        nvarchar(max)
    declare @timeout        int             = 4 * 100;
    declare @found          bit = 0
    declare @retry          int = 0
    declare @deal_status    nvarchar(4000)
    declare @response       nvarchar(max)
    
    while   @found = 0
    and     @retry < @timeout
    begin

        select  @found              = 1
        ,       @deal_status        = cr.deal_status
        ,       @response           =   (   select  *
                                            from    confirm_response j
                                            where   j.deal_reference    = cr.deal_reference
                                            for JSON path, Without_Array_Wrapper    )
        from    confirm_response    cr
        where   cr.deal_reference   = @deal_reference 
        
        waitfor delay '00:00:00:010'

        set @retry += 1

    end

    if @found = 1
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
        
        --      Assume api interface is not active or is in abnormal state. 
        --      Prevent the request will be untimely executed when interface is recovered by removing it from queue.
        delete  rrq
        from    rest_request_queue  rrq
        where   json_value(rrq.parameter, '$.dealReference') = @deal_reference

        set @message = 'Failed to receive confirmation for OTC request within reasonable time. Dealreference "' + @deal_reference + '".'
        raiserror(@message, 10, 10) with nowait

        return -2

    end

end

go