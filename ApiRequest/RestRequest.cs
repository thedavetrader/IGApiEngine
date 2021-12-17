using System.Diagnostics.CodeAnalysis;
using IGApi;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using IGApi.Common.Extensions;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public bool IsTradingRequest;

        public bool IsRestCall;

        public ApiRequestQueueItem RestRequestQueueItem;

        private readonly Task? _restRequestCallTask;

        private readonly ApiEngine _apiEngine = ApiEngine.Instance;

        public ApiRequestItem([NotNullAttribute] ApiRequestQueueItem restRequestQueueItem)
        {
            _ = _apiEngine.LoginSessionInformation ?? throw new Exception("Not, or no longer logged in. Check internet or IG Api service status.");

            RestRequestQueueItem = restRequestQueueItem;

            switch (RestRequestQueueItem.RestRequest)
            {
                case nameof(GetAccountDetails):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        _restRequestCallTask = new Task(() => GetAccountDetails());
                        break;
                    }
                case nameof(GetWorkingOrders):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        _restRequestCallTask = new Task(() => GetWorkingOrders());
                        break;
                    }
                case nameof(GetActivityHistory):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        _restRequestCallTask = new Task(() => GetActivityHistory());
                        break;
                    }
                case nameof(GetTransactionHistory):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        _restRequestCallTask = new Task(() => GetTransactionHistory());
                        break;
                    }
                case nameof(GetOpenPositions):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        _restRequestCallTask = new Task(() => GetOpenPositions());
                        break;
                    }
                case nameof(GetEpicDetails):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => GetEpicDetails(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(GetEpicDetails));
                        break;
                    }
                case nameof(GetClientSentiment):
                    {
                        IsTradingRequest = false;
                        IsRestCall = true;
                        _restRequestCallTask = new Task(() => GetClientSentiment());
                        break;
                    }                
                case nameof(CreatePosition):
                    {
                        IsTradingRequest = true;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => CreatePosition(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(CreatePosition));
                        break;
                    }
                case nameof(EditPosition):
                    {
                        IsTradingRequest = true;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => EditPosition(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(EditPosition));
                        break;
                    }
                case nameof(ClosePosition):
                    {
                        IsTradingRequest = true;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => ClosePosition(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(ClosePosition));
                        break;
                    }
                case nameof(CreateWorkingOrder):
                    {
                        IsTradingRequest = true;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => CreateWorkingOrder(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(CreateWorkingOrder));
                        break;
                    }
                case nameof(EditWorkingOrder):
                    {
                        IsTradingRequest = true;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => EditWorkingOrder(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(EditWorkingOrder));
                        break;
                    }
                case nameof(DeleteWorkingOrder):
                    {
                        IsTradingRequest = true;
                        IsRestCall = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => DeleteWorkingOrder(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(DeleteWorkingOrder));
                        break;
                    }
                default:
                    {
                        throw new Exception($"The restrequestcall \"{RestRequestQueueItem.RestRequest}\" is not implemented by class RestRequest.");
                    }
            }
        }

        public void Execute()
        {
            if (_restRequestCallTask is not null)
                _restRequestCallTask.FireAndForget();
            else
                Log.WriteLine($"Execution of the rest request {RestRequestQueueItem.RestRequest} is not implemented yet.");
        }

        private void RemoveObsoleteEpicTicks(IGApiDbContext iGApiDbContext)
        {
            _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

            var obsoleteEpicTicks = iGApiDbContext.EpicTicks
                    .ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                    .Where(w => !ApiEngine.EpicStreamPriceAvailableCheck(w.Epic));

            //  Remoe EpicTicks of which streaming prices are not available. These are leftovers from when a position was opened, despite that 
            //  at that time it could not be determined wether streamprices were available for that the particular epic of that position.
            iGApiDbContext.EpicTicks.RemoveRange(obsoleteEpicTicks);

            //  Remove from list
            _apiEngine.RemoveEpicStreamListItems(_apiEngine.EpicStreamList.Where(f => obsoleteEpicTicks.Any(s => s.Epic == f.Epic)));
        }             
    }
}
