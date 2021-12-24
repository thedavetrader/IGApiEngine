
go

create or alter procedure create_working_order
        @epic                       nvarchar(4000)   
,       @direction                  nvarchar(4)           
,       @size                       decimal(38,2)   
,       @currency_code              nvarchar(4000)  = null
,       @expiry                     nvarchar(4000)  = '-'
,       @good_till_date             datetime2       = null
,       @good_during_minutes        int             = null
,       @level                      decimal(38,19)
,       @guaranteed_stop            bit             = 0             
,       @stop_level                 decimal(38,19)  = null
,       @stop_distance              decimal(38,19)  = null
,       @limit_level                decimal(38,19)  = null
,       @limit_distance             decimal(38,19)  = null
as begin

    set nocount on
    
    /*      ************************************************************************************************
            SQL implementation of create working_order request.
            Ref: https://labs.ig.com/rest-trading-api-reference/service-detail?id=608
            ************************************************************************************************    */
    declare @message                nvarchar(4000)
    
    if not @direction in ('BUY', 'SELL')
        throw 51000, 'Parameter "@direction" only accepts values "BUY" or "SELL".', 1

    if @good_till_date is not null and @good_during_minutes is not null
        throw 51000, 'Set only one of @good_till_date or @good_during_minutes.', 1

    --  [Constraint: If guaranteedStop equals true, then set stopDistance]
    if @guaranteed_stop = 1 and @stop_distance is not null
        throw 51000, 'If guaranteedStop equals true, then set stopDistance', 1

    --  [Constraint: Set only one of {limitLevel,limitDistance}]
    if @limit_level is not null and @limit_distance is not null
        throw 51000, 'Set only one of {limitLevel,limitDistance}', 1
    
    --  [Constraint: Set only one of {stopLevel,stopDistance}]
    if @stop_level is not null and @stop_distance is not null
        throw 51000, 'Set only one of {stopLevel,stopDistance}', 1
        
    /*      ********************************
            Default 
            ********************************    */
    declare @min_deal_size          decimal(38,19)
    declare @time_in_force          nvarchar(4000)  
    declare @type                   nvarchar(4000)

    if @good_till_date is null and @good_during_minutes is not null
        set @good_till_date = dateadd(minute, @good_during_minutes, getutcdate())

    select  @min_deal_size  = ed.dealing_rule_value_min_deal_size
    from    epic_detail     ed
    where   ed.epic         = @epic

    select  @time_in_force          = iif(@good_till_date is null, 'GOOD_TILL_CANCELLED', 'GOOD_TILL_DATE')
    ,       @type                   = case 
                                          when @direction  = 'BUY'  and @level >  et.offer then 'STOP'
                                          when @direction  = 'BUY'  and @level <= et.offer then 'LIMIT'
                                          when @direction  = 'SELL' and @level >= et.bid   then 'LIMIT'
                                          when @direction  = 'SELL' and @level <  et.bid   then 'STOP'
                                      end
    from    epic_tick       et
    where   et.epic         = @epic

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
    
    if @type is null
    begin
        set @message = 'Price information could not be found for epic "' + @epic + '". Check if tick information is available for this epic or check api engine status.'
        ;throw 51000, @message, 1
    end

    /*      ********************************
            INIT
            ********************************    */
    declare @deal_reference nvarchar(36)    = right(convert(nvarchar(36), newid()), 30)
    declare @is_limit_level_valid   bit = 1
    declare @is_stop_level_valid    bit = 1

    select  @is_limit_level_valid   = case 
                                          when @direction  = 'BUY'  and @limit_level > @level then 1
                                          when @direction  = 'SELL' and @limit_level < @level then 1
                                          else 0
                                      end
    ,       @is_stop_level_valid    = case 
                                          when @direction  = 'BUY'  and @stop_level  < @level then 1
                                          when @direction  = 'SELL' and @stop_level  > @level then 1
                                          else 0
                                      end

    if @is_limit_level_valid = 0
    begin
        if @direction = 'BUY'
            set @message = 'Limitlevel (' + convert(nvarchar, @limit_level) + ') should be above the workingorder level (' + convert(nvarchar, @level) + ').'
        if @direction = 'SELL'
            set @message = 'Limitlevel (' + convert(nvarchar, @limit_level) + ') should be below the workingorder level (' + convert(nvarchar, @level) + ').'

        ;throw 51000, @message, 1
    end

    if @is_stop_level_valid = 0
    begin
        if @direction = 'BUY'
            set @message = 'Stoplevel (' + convert(nvarchar, @stop_level) + ') should be below the workingorder level (' + convert(nvarchar, @level) + ').'
        if @direction = 'SELL'
            set @message = 'Stoplevel (' + convert(nvarchar, @stop_level) + ') should be above the workingorder level (' + convert(nvarchar, @level) + ').'

        ;throw 51000, @message, 1
    end

    /*      ********************************
            Request for create working_order.    
            ********************************    */
    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap        )
    select  request             = 'CreateWorkingOrder'
    ,       parameter           =   (   select  currencyCode            = @currency_code
                                        ,       dealReference           = @deal_reference
                                        ,       direction               = @direction
                                        ,       epic                    = @epic
                                        ,       size                    = @size
                                        ,       expiry                  = @expiry
                                        ,       goodTillDate            = format(@good_till_date, 'yyyy/MM/dd HH:mm:ss')
                                        ,       guaranteedStop          = @guaranteed_stop
                                        ,       level                   = @level
                                        ,       limitLevel              = @limit_level
                                        ,       limitDistance           = @limit_distance
                                        ,       stopLevel               = @stop_level
                                        ,       stopDistance            = @stop_distance
                                        ,       timeInForce             = @time_in_force
                                        ,       type                    = @type
                                        for JSON path, Without_Array_Wrapper   )
    ,       execute_asap        = 1

    declare @check_result   int = 0
    
    exec    @check_result   = check_confirmation @deal_reference = @deal_reference

    return  @check_result

end

go

--exec create_working_order
--        @epic                       = 'CS.D.ETHUSD.CFE.IP'
--,       @direction                  = 'SELL'
--,       @size                       = 0.5
--,       @level                      = 4210
--,       @good_during_minutes        = 10
--,       @limit_level                = 4000
--,       @stop_level                 = 4500
--,       @good_till_date             = '2021/12/18 17:16:00'