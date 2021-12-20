create or alter procedure validate_stop_limit
        @epic                       nvarchar(4000)   
,       @direction                  nvarchar(4)           
,       @limit_level                decimal(38,19)  = null
,       @stop_level                 decimal(38,19)  = null
as begin
    declare @message                nvarchar(4000)
    declare @is_limit_level_valid   bit = 1
    declare @is_stop_level_valid    bit = 1
    declare @current_offer          decimal(38,19)
    declare @current_bid            decimal(38,19)

    select  @is_limit_level_valid   = case 
                                          when @limit_level is null                             then 1
                                          when @direction  = 'BUY'  and @limit_level > et.bid   then 1
                                          when @direction  = 'SELL' and @limit_level < et.offer then 1
                                          else 0
                                      end
    ,       @is_stop_level_valid    = case
                                          when @stop_level is null                              then 1
                                          when @direction  = 'BUY'  and @stop_level  < et.bid   then 1
                                          when @direction  = 'SELL' and @stop_level  > et.offer then 1
                                          else 0
                                      end
    ,       @current_offer          = et.offer
    ,       @current_bid            = et.bid
    from    epic_tick       et
    where   et.epic         = @epic

    if @is_limit_level_valid = 0
    begin
        if @direction = 'BUY'
            set @message = 'Limitlevel (' + convert(nvarchar, @limit_level) + ') should be above the current bid price (' + convert(nvarchar, @current_offer) + ').'
        if @direction = 'SELL'
            set @message = 'Limitlevel (' + convert(nvarchar, @limit_level) + ') should be below the current offer price (' + convert(nvarchar, @current_bid) + ').'

        ;throw 51000, @message, 1
    end

    if @is_stop_level_valid = 0
    begin
        if @direction = 'BUY'
            set @message = 'Stoplevel (' + convert(nvarchar, @stop_level) + ') should be below the current bid price (' + convert(nvarchar, @current_bid) + ').'
        if @direction = 'SELL'
            set @message = 'Stoplevel (' + convert(nvarchar, @stop_level) + ') should be above the current offer price (' + convert(nvarchar, @current_offer) + ').'

        ;throw 51000, @message, 1
    end

end
