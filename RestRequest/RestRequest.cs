using System.Diagnostics.CodeAnalysis;
using IGApi;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        public bool IsTradingRequest;

        public RestRequestQueueItem RestRequestQueueItem;

        private readonly Task? _restRequestCallTask;

        private readonly ApiEngine _apiEngine = ApiEngine.Instance;

        public RestRequest([NotNullAttribute] RestRequestQueueItem restRequestQueueItem)
        {
            _ = _apiEngine.LoginSessionInformation ?? throw new Exception("Not, or no longer logged in. Check internet or IG Api service status.");

            RestRequestQueueItem = restRequestQueueItem;

            switch (RestRequestQueueItem.RestRequest)
            {
                case nameof(GetAccountDetails):
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetAccountDetails());
                        break;
                    }
                case nameof(GetWorkingOrders):
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetWorkingOrders());
                        break;
                    }
                case nameof(GetOpenPositions):
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetOpenPositions());
                        break;
                    }
                case nameof(GetEpicDetails):
                    {
                        IsTradingRequest = false;
                        if (RestRequestQueueItem.Parameters is not null)
                            _restRequestCallTask = new Task(() => GetEpicDetails(RestRequestQueueItem.Parameters));
                        else
                            throw new InvalidRestRequestMissingParametersException(nameof(GetEpicDetails));
                        break;
                    }
                default:
                    {
                        throw new Exception($"The restrequestcall is not implemented by class RestRequest.");
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

            //  Remoe EpicTicks of which streaming prices are not available. These are leftovers from when a position was opened, despite that 
            //  at that time it could not be determined wether streamprices were available for that the particular epic of that position.
            iGApiDbContext.EpicTicks.RemoveRange(
                iGApiDbContext.EpicTicks
                    .ToList()   // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                    .Where(w => !ApiEngine.EpicStreamPriceAvailableCheck(w.Epic)));

            //  Remove from list
            var existingEpicStreamListItem = _apiEngine.EpicStreamList.Where(f => iGApiDbContext.EpicTicks.Any(s => s.Epic == f.Epic));
            _apiEngine.RemoveEpicStreamListItems(existingEpicStreamListItem);
        }

        private IgResponse<T>? RestApiClientCall<T>(Task<IgResponse<T>> callTask)
        {
            IgResponse<T>? response = null;

            int retryCount = ApiEngine.AllowedApiCallsPerMinute;

            bool retry = true;

            while (retry)
            {
                response = Task.Run(async () => await callTask).Result;

                _ = response ?? throw new RestCallNullReferenceException(nameof(response));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    retry = false;
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && retryCount >= 0)    // possible temporary reach of api call limit.
                {
                    Utility.DelayAction(1000 * ApiEngine.CycleTime);
                    retry = true;
                }
                else
                {
                    retry = false;
                    throw new RestCallHttpRequestException(nameof(RestApiClientCall), response.StatusCode);
                }

                retryCount--;
            }

            return response;

        }
    }
}
