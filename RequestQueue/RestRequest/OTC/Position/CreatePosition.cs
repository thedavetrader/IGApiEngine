using IGApi.Common;
using IGApi.Model;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? CreatePositionCompleted;
        public void CreatePosition(string positionRequest)
        {
            try
            {
                dto.endpoint.positions.create.otc.v2.CreatePositionRequest createPositionRequest = JsonConvert.DeserializeObject<dto.endpoint.positions.create.otc.v2.CreatePositionRequest>(positionRequest);

                var response = _apiEngine.IGRestApiClient.createPositionV2(createPositionRequest).UseManagedCall();

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
                CreatePositionCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}