using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? CreatePositionCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: true)]
        public void CreatePosition()
        {
            try
            {
                _ = ApiRequestQueueItem.Parameters ?? throw new InvalidRequestMissingParametersException();
                string request = ApiRequestQueueItem.Parameters;

                dto.endpoint.positions.create.otc.v2.CreatePositionRequest createPositionRequest = JsonConvert.DeserializeObject<dto.endpoint.positions.create.otc.v2.CreatePositionRequest>(request);

                var response = _apiEngine.IGRestApiClient.createPositionV2(createPositionRequest).UseManagedCall();

                checkConfirmationReceived(response.Response.dealReference);

                RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetOpenPositions), executeAsap: true, isRecurrent: false, Guid.NewGuid(), cancellationToken: _cancellationToken);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(CreatePositionCompleted);
            }
        }
    }
}