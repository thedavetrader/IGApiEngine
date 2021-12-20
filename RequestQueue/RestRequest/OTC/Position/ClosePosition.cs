using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? ClosePositionCompleted;
        public void ClosePosition(string positionRequest)
        {
            try
            {
                dto.endpoint.positions.close.v1.ClosePositionRequest closePositionRequest = JsonConvert.DeserializeObject<dto.endpoint.positions.close.v1.ClosePositionRequest>(positionRequest);

                var response = _apiEngine.IGRestApiClient.closePosition(closePositionRequest).UseManagedCall();

                if (response is not null)
                    checkConfirmationReceived(response.Response.dealReference);
                else
                    throw new RestCallNullReferenceException();
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                ClosePositionCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}