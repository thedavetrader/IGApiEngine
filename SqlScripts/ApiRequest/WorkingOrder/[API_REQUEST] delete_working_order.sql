
go

create or alter procedure delete_working_order
        @deal_id                    nvarchar(64)   
as begin

    set nocount on

    /*      ************************************************************************************************
            SQL implementation of create working_order request.
            Ref: https://labs.ig.com/rest-trading-api-reference/service-detail?id=592
            ************************************************************************************************    */
    declare @message        nvarchar(4000)

    /*      ********************************
            Default
            ********************************    */   
    set @deal_id = (select  op.deal_id
                    from    working_order   op 
                    where   op.deal_id          = @deal_id )
                            
    
    if @deal_id is null
    begin
        set @message = 'Dealid could not be found. Possibly no working_order for this epic.'
        ;throw 51000, @message, 1
    end

    /*      ********************************
            INIT
            ********************************    */

    /*      ********************************
            Request for create working_order.    
            ********************************    */
    insert  api_request_queue_item(
            request
    ,       parameter
    ,       execute_asap        )
    select  request             = 'DeleteWorkingOrder'
    ,       parameter           =   (   select  dealId      = @deal_id
                                        for JSON path, Without_Array_Wrapper   )
    ,       execute_asap        = 1

    declare @check_result   int = 0
    
    exec    @check_result   = check_confirmation @deal_id = @deal_id

    return  @check_result

end

go

--exec    delete_working_order
--        @deal_id        = 'DIAAAAHACA7EPAZ'