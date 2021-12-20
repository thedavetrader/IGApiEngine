create or alter procedure close_position
        @deal_reference             nvarchar(36)    = null
,       @deal_id                    nvarchar(64)    = null
,       @direction                  nvarchar(4)     = null  
,       @epic                       nvarchar(4000)  = null
,       @expiry                     nvarchar(4000)  = '-'
,       @level                      decimal(38,19)  = null
,       @order_type                 nvarchar(4000)  = 'MARKET'
,       @quote_id                   nvarchar(4000)  = null
,       @size                       decimal(38,2)   = null
,       @time_in_force              nvarchar(4000)  = 'EXECUTE_AND_ELIMINATE'

as begin

    set nocount on
    
    /*      ************************************************************************************************
            SQL implementation of create position request.
            Ref: https://labs.ig.com/rest-trading-api-reference/service-detail?id=608
            ************************************************************************************************    */

    declare @message        nvarchar(4000)
    declare @min_deal_size  decimal(38,19)
    
    if not @direction in ('BUY', 'SELL')
        throw 51000, 'Parameter "@direction" only accepts values "BUY" or "SELL".', 1

    if not @order_type in ('LIMIT', 'MARKET', 'QUOTE')
        throw 51000, 'Parameter "@order_type" only accepts values "LIMIT", "MARKET" or "QUOTE".', 1
        
    if not @time_in_force in ('EXECUTE_AND_ELIMINATE', 'FILL_OR_KILL')
        throw 51000, 'Parameter "@time_in_force" only accepts values "LIMITEXECUTE_AND_ELIMINATE" or "FILL_OR_KILL".', 1

    --  [Constraint: If epic is defined, then set expiry]
    if  @epic is not null and @expiry is null
        throw 51000, 'If epic is defined, then set expiry', 1

    --  [Constraint: If orderType equals LIMIT, then DO NOT set quoteId]
    if @order_type = 'LIMIT' and @quote_id is not null
        throw 51000, 'If orderType equals LIMIT, then DO NOT set quoteId', 1

    --  [Constraint: If orderType equals LIMIT, then set level]
    if @order_type = 'LIMIT' and @level is null
        throw 51000, 'If orderType equals LIMIT, then set level', 1

    --  [Constraint: If orderType equals MARKET, then DO NOT set level,quoteId]
    if @order_type = 'MARKET' and (@level is not null or @quote_id is not null)
        throw 51000, 'If orderType equals MARKET, then DO NOT set level,quoteId', 1

    --  [Constraint: If orderType equals QUOTE, then set level,quoteId]
    if @order_type = 'QUOTE' and @quote_id is null
        throw 51000, 'If orderType equals QUOTE, then set level,quoteId', 1

    --  [Constraint: Set only one of {dealId,epic}]
    if @epic is not null and @deal_id is not null
        throw 51000, 'Set only one of {dealId,epic}', 1

    /*      ********************************
            Default
            ********************************    */   
    select  @deal_reference = op.deal_reference
    ,       @deal_id        = op.deal_id
    ,       @epic           = op.epic
    ,       @size           = coalesce(@size        , op.size                                                             )
    ,       @direction      = coalesce(@direction   , case op.direction when 'BUY' then 'SELL' when 'SELL' then 'BUY' end )
    from    open_position   op
    where   (       op.deal_id          = @deal_id
                or  op.epic             = @epic 
                or  op.deal_reference   = @deal_reference   )
    
    if @deal_reference is null
    begin
        set @message = 'Dealreference could not be found for epic "' + coalesce(@epic, 'NULL') + '". Possible no open position for this epic.'
        ;throw 51000, @message, 1
    end

    if @deal_id is null
    begin
        set @message = 'Dealid could not be found for epic "' + coalesce(@epic, 'NULL') + '". Possible no open position for this epic.'
        ;throw 51000, @message, 1
    end

    if @direction is null
    begin
        set @message = 'Direction could not be found for epic "' + coalesce(@epic, 'NULL') + '". Possible no open position for this epic.'
        ;throw 51000, @message, 1
    end

    if @size is null
    begin
        set @message = 'Size could not be found for epic "' + coalesce(@epic, 'NULL') + '". Possibly no open position for this epic. Alternatively provide the size with the parameter "@size".'
        ;throw 51000, @message, 1
    end

    /*      ********************************
            INIT
            ********************************    */

    /*      ********************************
            Request for create position.    
            ********************************    */
    insert  api_request_queue_item(
            request
    ,       parameter
    ,       execute_asap        )
    select  request             = 'ClosePosition'
    ,       parameter           =   (   select  dealId      = @deal_id
                                        ,       direction   = @direction
                                        ,       epic        = iif(@deal_id is not null, null, @epic)    -- Prefer @deal_id above @epic.
                                        ,       expiry      = @expiry
                                        ,       level       = @level
                                        ,       orderType   = @order_type
                                        ,       quoteId     = @quote_id
                                        ,       size        = @size
                                        ,       timeInForce = @time_in_force
                                        for JSON path, Without_Array_Wrapper   )
    ,       execute_asap        = 1

    declare @check_result   int = 0
    
    exec    @check_result   = check_confirmation @deal_reference = @deal_reference

    return  @check_result

end

go

exec    close_position
        @deal_reference = 'F8-F4CA-460C-9EA6-BCD6CEE482BB'
--,       @deal_id        = 
--,       @direction      = 
--,       @epic           = 
--,       @expiry         = 
--,       @level          = 
--,       @order_type     = 
--,       @quote_id       = 
,       @size           = 0.01  -- Partial close
--,       @time_in_force  = 
