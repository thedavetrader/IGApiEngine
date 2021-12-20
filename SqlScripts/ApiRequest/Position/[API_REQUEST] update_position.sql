create or alter procedure edit_position

        @deal_id                    nvarchar(64)    
,       @guaranteed_stop            bit              = 0             
,       @limit_level                decimal(38,19)   = null
,       @stop_level                 decimal(38,19)   = null
,       @trailing_stop              bit              = 0
,       @trailing_stop_distance     decimal(38,19)   = null
,       @trailing_stop_increment    decimal(38,19)   = null
as begin

    set nocount on
    
    /*      ************************************************************************************************
            SQL implementation of create position request.
            Ref: https://labs.ig.com/rest-trading-api-reference/service-detail?id=608
            ************************************************************************************************    */

    declare @message        nvarchar(4000)
    declare @min_deal_size  decimal(38,19)

    --  [Constraint: If guaranteedStop equals true, then set stopLevel]
    if @guaranteed_stop = 1 and @stop_level is not null
        throw 51000, 'If guaranteedStop equals true, then set stopLevel', 1
    
    --  [Constraint: If guaranteedStop equals true, then trailingStop must be false]
        if @guaranteed_stop = 1 and @trailing_stop = 1
        throw 51000, 'If guaranteedStop equals true, then trailingStop must be false', 1

    --  [Constraint: If trailingStop equals false, then DO NOT set trailingStopDistance,trailingStopIncrement]
    if @trailing_stop = 0 and (@trailing_stop_distance is not null or @trailing_stop_increment is not null)
        throw 51000, 'If trailingStop equals false, then DO NOT set trailingStopDistance, trailingStopIncrement', 1

    --  [Constraint: If trailingStop equals true, then guaranteedStop must be false]
    if @trailing_stop = 1 and @guaranteed_stop = 1
        throw 51000, 'If trailingStop equals true, then guaranteedStop must be false', 1

    --  [Constraint: If trailingStop equals true, then set trailingStopDistance,trailingStopIncrement,stopLevel]
    if @trailing_stop = 1 and (@trailing_stop_distance is not null or @trailing_stop_increment is not null)
        throw 51000, 'If trailingStop equals true, then set trailingStopDistance,trailingStopIncrement,stopLevel', 1

    
    /*      ********************************
            Default
            ********************************    */

    /*      ********************************
            INIT
            ********************************    */
    declare @deal_reference nvarchar(36)    
    declare @epic           nvarchar(4000)
    declare @direction      nvarchar(4000)
    
    select  @deal_reference             = op.deal_reference
    ,       @limit_level                = coalesce(@limit_level            , op.limit_level           ) -- Use value "0" to disable
    ,       @stop_level                 = coalesce(@stop_level             , op.stop_level            ) -- Use value "0" to disable
    ,       @trailing_stop              = coalesce(@trailing_stop          , t.trailing_stop          )
    ,       @trailing_stop_distance     = coalesce(@trailing_stop_distance , op.trailing_stop_distance)
    ,       @trailing_stop_increment    = coalesce(@trailing_stop_increment, op.trailing_step         )
    ,       @epic                       = op.epic
    ,       @direction                  = op.direction
    from    open_position   op
    cross
    apply   (   select  trailing_stop = cast(iif(op.trailing_stop_distance is not null and op.trailing_step is not null, 1, 0) as bit) ) t
    where   op.deal_id          = @deal_id            

    if @deal_reference is null
    begin
        set @message = 'Dealreference could not be found for dealid"' + coalesce(@deal_id, 'NULL') + '". Possible no open position for this epic.'
        ;throw 51000, @message, 1
    end

    exec    validate_stop_limit
            @epic           = @epic        
    ,       @direction      = @direction   
    ,       @limit_level    = @limit_level 
    ,       @stop_level     = @stop_level  

    /*      ********************************
            Request for edit position.    
            ********************************    */
    insert  api_request_queue_item (
            request
    ,       parameter
    ,       execute_asap        )
    select  request             = 'EditPosition'
    ,       parameter           =   (   select  dealId                                      = @deal_id                 
                                        ,       'EditPositionRequest.limitLevel'            = @limit_level                  
                                        ,       'EditPositionRequest.stopLevel'             = @stop_level              
                                        ,       'EditPositionRequest.trailingStop'          = @trailing_stop          
                                        ,       'EditPositionRequest.trailingStopDistance'  = @trailing_stop_distance 
                                        ,       'EditPositionRequest.trailingStopIncrement' = @trailing_stop_increment
                                        for JSON path, Without_Array_Wrapper   )
    ,       execute_asap        = 1

    declare @check_result   int = 0
    
    exec    @check_result   = check_confirmation @deal_reference = @deal_reference

    return  @check_result

end

go

exec    edit_position
        @deal_id                    = 'DIAAAAG9UCFNVAL'
,       @limit_level                = 4500
,       @stop_level                 = 0
--,       @guaranteed_stop            
--,       @trailing_stop           
--,       @trailing_stop_distance  
--,       @trailing_stop_increment 
