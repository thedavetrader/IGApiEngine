using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? DeleteWorkingOrderCompleted;

        private class CustomDeleteWorkingOrderRequest
        {
            public string? DealId { get; set; }

            public dto.endpoint.workingorders.delete.v1.DeleteWorkingOrderRequest? DeleteWorkingOrderRequest { get; set; } = new();
        }

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void DeleteWorkingOrder()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                CustomDeleteWorkingOrderRequest deleteWorkingOrderRequest = JsonConvert.DeserializeObject<CustomDeleteWorkingOrderRequest>(request);

                if (!string.IsNullOrEmpty(deleteWorkingOrderRequest.DealId))    //DeleteWorkingOrderRequest has no members, so is allowed to be null.
                {
                    var response = _apiEngine.IGRestApiClient.deleteWorkingOrder(deleteWorkingOrderRequest.DealId, deleteWorkingOrderRequest.DeleteWorkingOrderRequest).UseManagedCall();

                    checkConfirmationReceived(response.Response.dealReference);

                    RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetWorkingOrders), executeAsap: true, isRecurrent: false, Guid.NewGuid(), cancellationToken: _cancellationToken);
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(DeleteWorkingOrderCompleted);
            }
        }
    }
}