
go

create or alter procedure edit_working_order
        @deal_id                    nvarchar(64)    
,       @time_in_force              nvarchar(4000)  = null
,       @good_till_date             datetime2       = null
,       @good_during_minutes        int             = null
,       @guaranteed_stop            bit             = 0             
,       @level                      decimal(38,19)  = null
,       @limit_level                decimal(38,19)  = null
,       @limit_distance             decimal(38,19)  = null
,       @stop_level                 decimal(38,19)  = null
,       @stop_distance              decimal(38,19)  = null
as begin

    set nocount on
    
    /*      ************************************************************************************************
            SQL implementation of create working_order request.
            Ref: https://labs.ig.com/rest-trading-api-reference/service-detail?id=592
            ************************************************************************************************    */

    declare @message        nvarchar(4000)
    declare @min_deal_size  decimal(38,19)

    if @time_in_force is not null and @time_in_force not in ('GOOD_TILL_CANCELLED', 'GOOD_TILL_DATE')
        throw 51000, 'Parameter "@time_in_force" only accepts values "GOOD_TILL_CANCELLED" or "GOOD_TILL_DATE".', 1

    if  @time_in_force = 'GOOD_TILL_DATE' and @good_till_date is null
        throw 51000, 'When parameter "@time_in_force" is "GOOD_TILL_DATE", parameter @good_till_date should also be set.', 1

    if  @time_in_force = 'GOOD_TILL_DATE' and @good_till_date < getutcdate()
        throw 51000, 'Parameter @good_till_date should be set to a time in the future.', 1       

    --  [Constraint: If guaranteedStop equals true, then set stopLevel]
    if @guaranteed_stop = 1 and @stop_level is not null
        throw 51000, 'If guaranteedStop equals true, then set stopLevel', 1
    
    --  [Constraint: Set only one of {limitLevel,limitDistance}]
    if @limit_level is not null and @limit_distance is not null
        throw 51000, 'Set only one of {limitLevel,limitDistance}', 1
    
    --  [Constraint: Set only one of {stopLevel,stopDistance}]
    if @stop_level is not null and @stop_distance is not null
        throw 51000, 'Set only one of {stopLevel,stopDistance}', 1
    
    /*      ********************************
            Default
            ********************************    */
    declare @type           nvarchar(4000)
    declare @epic           nvarchar(4000)
    declare @direction      nvarchar(4000)

    if @good_till_date is null and @good_during_minutes is not null
        set @good_till_date = dateadd(minute, @good_during_minutes, getutcdate())

    select  @good_till_date     = coalesce(@good_till_date  , wo.good_till_date     )   -- Use value ??? to disable
    ,       @limit_distance     = coalesce(@limit_distance  , wo.limit_distance     )   -- Use value "0" to disable
    ,       @stop_distance      = coalesce(@stop_distance   , wo.stop_distance      )   -- Use value "0" to disable
    ,       @type               = case 
                                     when wo.direction  = 'BUY'  and @level >  et.offer then 'STOP'
                                     when wo.direction  = 'BUY'  and @level <= et.offer then 'LIMIT'
                                     when wo.direction  = 'SELL' and @level >= et.bid   then 'LIMIT'
                                     when wo.direction  = 'SELL' and @level <  et.bid   then 'STOP'
                                 end
    ,       @epic               = wo.epic
    ,       @direction          = wo.direction
    from    working_order       wo
    join    epic_tick           et
    on      et.epic             = wo.epic
    where   wo.deal_id          = @deal_id

    set     @time_in_force      = coalesce(@time_in_force, iif(@good_till_date is null, 'GOOD_TILL_CANCELLED', 'GOOD_TILL_DATE'))

    if @direction is null or @epic is null
    begin
        set @message = 'No working order found for this deal id.'
        ;throw 51000, @message, 1
    end

    exec    validate_stop_limit
            @epic           = @epic        
    ,       @direction      = @direction   
    ,       @limit_level    = @limit_level 
    ,       @stop_level     = @stop_level  

    /*      ********************************
            Request for edit working_order.    
            ********************************    */
    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap        )
    select  request             = 'EditWorkingOrder'
    ,       parameter           =   (   select  dealId                                  = @deal_id                 
                                        ,       'EditWorkingOrderRequest.goodTillDate'  = format(@good_till_date, 'yyyy/MM/dd HH:mm:ss')
                                        ,       'EditWorkingOrderRequest.level'         = @level
                                        ,       'EditWorkingOrderRequest.limitDistance' = @limit_distance
                                        ,       'EditWorkingOrderRequest.limitLevel'    = @limit_level
                                        ,       'EditWorkingOrderRequest.stopLevel'     = @stop_level
                                        ,       'EditWorkingOrderRequest.stopDistance'  = @stop_distance
                                        ,       'EditWorkingOrderRequest.timeInForce'   = @time_in_force
                                        ,       'EditWorkingOrderRequest.type'          = @type
                                        for JSON path, Without_Array_Wrapper   )
    ,       execute_asap        = 1

    declare @check_result   int = 0
    
    exec    @check_result   = check_confirmation @deal_id = @deal_id

    return  @check_result

end

go

--exec    edit_working_order
--        @deal_id                    = 'DIAAAAHACAN36AH'
--,       @level                      = 3800
--,       @time_in_force              = 'GOOD_TILL_CANCELLED'
--,       @good_during_minutes        = 1
--,       @limit_level                = 3000
--,       @limit_distance             = 50
--,       @stop_level                 = 0

--,       @guaranteed_stop            
--,       @trailing_stop           
--,       @trailing_stop_distance  
--,       @trailing_stop_increment 
