
go

create or alter procedure create_position
        @epic                       nvarchar(4000)   
,       @direction                  nvarchar(4)           
,       @size                       decimal(38,2)   
,       @order_type                 nvarchar(4000)   = 'MARKET'
,       @currency_code              nvarchar(4000)   = null
,       @force_open                 bit              = 1        
,       @expiry                     nvarchar(4000)   = '-'
,       @level                      decimal(38,19)   = null
,       @guaranteed_stop            bit              = 0             
,       @stop_level                 decimal(38,19)   = null
,       @stop_distance              decimal(38,19)   = null
,       @trailing_stop              bit              = 0
,       @trailing_stop_increment    decimal(38,19)   = null
,       @limit_level                decimal(38,19)   = null
,       @limit_distance             decimal(38,19)   = null
,       @quote_id                   nvarchar(4000)   = null

as begin

    set nocount on
    
    /*      ************************************************************************************************
            SQL implementation of create position request.
            Ref: https://labs.ig.com/rest-trading-api-reference/service-detail?id=608
            ************************************************************************************************    */

    declare @message        nvarchar(4000)
    
    if not @direction in ('BUY', 'SELL')
        throw 51000, 'Parameter "@direction" only accepts values "BUY" or "SELL".', 1

    if not @order_type in ('LIMIT', 'MARKET', 'QUOTE')
        throw 51000, 'Parameter "@order_type" only accepts values "LIMIT", "MARKET" or "QUOTE".', 1

    --  [Constraint: If a limitDistance is set, then forceOpen must be true]
    if @limit_distance is not null and @force_open = 0
        throw 51000, 'If limitDistance is set, then forceOpen must be true', 1
    
    --  [Constraint: If a limitLevel is set, then forceOpen must be true]
    if @limit_level is not null and @force_open = 0
        throw 51000, 'If a limitLevel is set, then forceOpen must be true', 1
    
    --  [Constraint: If a stopDistance is set, then forceOpen must be true]
    if @stop_distance is not null and @force_open = 0
        throw 51000, 'If stopDistance is set, then forceOpen must be true', 1

    --  [Constraint: If a stopLevel is set, then forceOpen must be true]
    if @stop_level is not null and @force_open = 0
        throw 51000, 'If a stopLevel is set, then forceOpen must be true', 1

    --  [Constraint: If guaranteedStop equals true, then set only one of stopLevel,stopDistance]
    if @guaranteed_stop = 1 and @stop_level is not null and @stop_distance is not null
        throw 51000, 'If guaranteedStop equals true, then set only one of stopLevel,stopDistance', 1

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
    
    --  [Constraint: If trailingStop equals false, then DO NOT set trailingStopIncrement]
    if @trailing_stop = 0 and @trailing_stop_increment is not null
        throw 51000, 'If trailingStop equals false, then DO NOT set trailingStopIncrement', 1

    --  [Constraint: If trailingStop equals true, then DO NOT set stopLevel]
    if @trailing_stop =1 and @stop_level is not null
        throw 51000, 'If trailingStop equals true, then DO NOT set stopLevel', 1
    
    --  [Constraint: If trailingStop equals true, then guaranteedStop must be false]
    if @trailing_stop = 1 and @guaranteed_stop = 1
        throw 51000, 'If trailingStop equals true, then guaranteedStop must be false', 1
    
    --  [Constraint: If trailingStop equals true, then set stopDistance,trailingStopIncrement]
    if @trailing_stop = 1 and (@stop_distance is null or @trailing_stop_increment is null)
        throw 51000, 'If trailingStop equals true, then set stopDistance,trailingStopIncrement', 1
    
    --  [Constraint: Set only one of {limitLevel,limitDistance}]
    if @limit_level is not null and @limit_distance is not null
        throw 51000, 'Set only one of {limitLevel,limitDistance}', 1
    
    --  [Constraint: Set only one of {stopLevel,stopDistance}]
    if @stop_level is not null and @stop_distance is not null
        throw 51000, 'Set only one of {stopLevel,stopDistance}', 1

    /*      ********************************
            Default
            ********************************    */
    declare @min_deal_size  decimal(38,19)

    select  @min_deal_size  = ed.dealing_rule_value_min_deal_size
    from    epic_detail     ed
    where   ed.epic         = @epic

    if @currency_code is null
        select  @currency_code  = dbo.get_epic_currency_code(@epic)

    if  @min_deal_size is null
    begin
        set @message = 'Minimum dealsize information could not be found for epic "' + @epic + '".'
        ;throw 51000, @message, 1
    end

    if  @min_deal_size is not null and @size < @min_deal_size
    begin
        set @message = 'Size "' + convert(nvarchar, @size) + '" is less than minimum deal size "' + convert(nvarchar, @min_deal_size) + '".'
        ;throw 51000, @message, 1
    end
    
    if @currency_code is null
    begin
        set @message = 'Currency information could not be found for epic "' + @epic + '". Alternatively provide the currencycode (e.g. "USD") with the parameter "@currency_code".'
        ;throw 51000, @message, 1
    end

    /*      ********************************
            INIT
            ********************************    */
    declare @deal_reference nvarchar(36)    = right(convert(nvarchar(36), newid()), 30)
    
    exec    validate_stop_limit
            @epic           = @epic        
    ,       @direction      = @direction   
    ,       @limit_level    = @limit_level 
    ,       @stop_level     = @stop_level  

    /*      ********************************
            Request for create position.    
            ********************************    */
    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap        )
    select  request             = 'CreatePosition'
    ,       parameter           =   (   select  epic                    = @epic                   
                                        ,       direction               = @direction              
                                        ,       size                    = @size
                                        ,       orderType               = @order_type             
                                        ,       currencyCode            = @currency_code          
                                        ,       forceOpen               = @force_open             
                                        ,       expiry                  = @expiry                 
                                        ,       level                   = @level                  
                                        ,       guaranteedStop          = @guaranteed_stop        
                                        ,       stopLevel               = @stop_level             
                                        ,       stopDistance            = @stop_distance          
                                        ,       trailingStop            = @trailing_stop          
                                        ,       trailingStopIncrement   = @trailing_stop_increment
                                        ,       limitLevel              = @limit_level            
                                        ,       limitDistance           = @limit_distance         
                                        ,       quoteId                 = @quote_id         
                                        ,       dealReference           = @deal_reference
                                        for JSON path, Without_Array_Wrapper   )
    ,       execute_asap        = 1

    declare @check_result   int = 0
    
    exec    @check_result   = check_confirmation @deal_reference = @deal_reference

    return  @check_result

end

go

--exec create_position
--        @epic                       = 'CS.D.ETHUSD.CFE.IP'
--,       @direction                  = 'BUY'
--,       @size                       = 50.5
--,       @order_type                 = 
--,       @currency_code              = 
--,       @force_open                 = 
--,       @expiry                     = 
--,       @level                      = 
--,       @guaranteed_stop            = 
--,       @stop_level                 = 5000
--,       @stop_distance              = 
--,       @trailing_stop              = 
--,       @trailing_stop_increment    = 
--,       @limit_level                = 
--,       @limit_distance             = 
--,       @quote_id                   = 