go

create or alter procedure get_epic_detail
	@epics epic_list	readonly
as begin

	if dbo.get_api_engine_status_is_alive() = 0
			throw 51000, 'Api enginen and/or requestqueuengine is not running or fully initialized.', 1

	insert	api_request_queue_item (
			request
	,		parameter
	,		execute_asap	)
	select	request			= 'GetEpicDetails'
	,		parameter		= (	select	epic
								from	@epics
								for json path	)
	,		execute_asap	= 1

end

go

--delete from epic_detail

declare	@epics epic_list

insert @epics (epic)
select	--top 60
epic		= td.broker_ticker_reference
from	tdt_live..ticker_detail	td
--where	td.broker_ticker_reference = 'CC.D.BO.UME.IP'

exec get_epic_detail @epics = @epics