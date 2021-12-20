use tdbt

go

create or alter function get_api_engine_status_is_alive()
returns bit
as begin
  return (  select  top 1
                    iif(datediff(millisecond, is_alive, getutcdate()) > 100, cast(0 as bit), cast(1 as bit))
            from    api_engine_status )
end

go

select dbo.get_api_engine_status_is_alive();