
go

create or alter function get_epic_currency_code(@epic nvarchar(128))
returns nvarchar(4000)
as begin

    return (    select  top 1 
                        edc.code
                from    epic_detail_currency    edc 
                cross
                apply   (   select  priority =  case edc.code
                                                    when 'EUR'  then 0
                                                    when 'USD'  then 1
                                                                else 2
                                                end )p
                where   edc.epic                = @epic 
                order 
                by      p.priority asc  )
end

go
