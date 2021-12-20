using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGApi.Common.Extensions;
using IGApi.Model;

namespace IGApi.RequestQueue
{
    using static Log;

    public partial class RequestQueueItem
    {
        public bool IsTradingRequest;

        public bool IsRestRequest;

        public ApiRequestQueueItem RestRequestQueueItem;

        private readonly Task? _restRequestCallTask;

        private readonly ApiEngine _apiEngine = ApiEngine.Instance;

        private readonly string _currentAccountId;

        public RequestQueueItem([NotNullAttribute] ApiRequestQueueItem restRequestQueueItem)
        {
            if (!_apiEngine.IsLoggedIn) throw new IGApiConncectionError();

            RestRequestQueueItem = restRequestQueueItem;

            _currentAccountId = _apiEngine.LoginSessionInformation.currentAccountId;

            switch (RestRequestQueueItem.Request)
            {
                case nameof(GetAccountDetails):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetAccountDetails());
                        break;
                    }
                case nameof(GetWorkingOrders):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetWorkingOrders());
                        break;
                    }
                case nameof(GetActivityHistory):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetActivityHistory());
                        break;
                    }
                case nameof(GetTransactionHistory):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetTransactionHistory());
                        break;
                    }
                case nameof(GetOpenPositions):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetOpenPositions());
                        break;
                    }
                case nameof(GetEpicDetails):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => GetEpicDetails(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(GetEpicDetails));
                        break;
                    }
                case nameof(GetClientSentiment):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetClientSentiment());
                        break;
                    }                
                case nameof(CreatePosition):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => CreatePosition(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(CreatePosition));
                        break;
                    }
                case nameof(EditPosition):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => EditPosition(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(EditPosition));
                        break;
                    }
                case nameof(ClosePosition):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => ClosePosition(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(ClosePosition));
                        break;
                    }
                case nameof(CreateWorkingOrder):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => CreateWorkingOrder(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(CreateWorkingOrder));
                        break;
                    }
                case nameof(EditWorkingOrder):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => EditWorkingOrder(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(EditWorkingOrder));
                        break;
                    }
                case nameof(DeleteWorkingOrder):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => DeleteWorkingOrder(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(DeleteWorkingOrder));
                        break;
                    }                
                case nameof(GetWatchlists):
                    {
                        IsTradingRequest = false;
                        IsRestRequest = true;
                        _restRequestCallTask = new Task(() => GetWatchlists());
                        break;
                    }
                case nameof(CreateWatchlist):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => CreateWatchlist(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(CreateWatchlist));
                        break;
                    }
                case nameof(DeleteWatchlist):
                    {
                        IsTradingRequest = true;
                        IsRestRequest = true;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => DeleteWatchlist(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(DeleteWatchlist));
                        break;
                    }
                default:
                    {
                        throw new Exception($"The restrequestcall \"{RestRequestQueueItem.Request}\" is not implemented by class RestRequest.");
                    }
            }
        }

        public void Execute()
        {
            if (_restRequestCallTask is not null)
                _restRequestCallTask.FireAndForget();
            else
                WriteLog(Messages($"Execution of the rest request {RestRequestQueueItem.Request} is not implemented yet."));
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
