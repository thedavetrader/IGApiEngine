using System.Diagnostics.CodeAnalysis;
using IGApi;
using IGApi.Model;
using IGApi.Common;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        public bool IsTradingRequest;

        public RestRequestQueue RestRequestQueueItem;

        private readonly Task? _restRequestCallTask;

        private readonly ApiEngine _iGApiEngine = ApiEngine.Instance;

        public RestRequest([NotNullAttribute] RestRequestQueue restRequestQueueItem)
        {
            _ = _iGApiEngine.LoginSessionInformation ?? throw new Exception("Not, or no longer logged in. Check internet or IG Api service status.");
            RestRequestQueueItem = restRequestQueueItem;

            switch (RestRequestQueueItem.RestRequest)
            {
                case "GetAccountDetails":
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetAccountDetails());
                        break;
                    }
                case "GetOpenPositions":
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetOpenPositionDetails());
                        break;
                    }
                case "CreatePosition":
                    {
                        IsTradingRequest = true;
                        //_restRequestCallTask = Task.CompletedTask;
                        break;
                    }
                case "GetEpicDetails":
                    {
                        IsTradingRequest = false;
                        _restRequestCallTask = new Task(() => GetEpicDetails(RestRequestQueueItem.Parameters));
                        break;
                    }
                    
                default:
                    {
                        throw new Exception($"The restrequestcall is invalid. The check constraint on column RestRequest of table IGRestRequestQueue is possibly corrupt or missing.");
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
    }
}
