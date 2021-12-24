
go

create or alter function indent_json_object(@json nvarchar(max))
returns nvarchar(max)
as begin

  declare @newline  nvarchar(2) = char(13)+char(10)

  return  replace(
          replace(
          replace(
          replace(@json , '{' , '{'   + @newline + '  ' )
                        , ',"', ','   + @newline + '  "')
                        , '}' ,         @newline + '}'  )
                        , '},', '},'  + @newline        )

end

go
