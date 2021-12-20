using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? CreateWorkingOrderCompleted;
        public void CreateWorkingOrder(string WorkingOrderRequest)
        {
            try
            {
                dto.endpoint.workingorders.create.v2.CreateWorkingOrderRequest createWorkingOrderRequest = JsonConvert.DeserializeObject<dto.endpoint.workingorders.create.v2.CreateWorkingOrderRequest>(WorkingOrderRequest);

                var response = _apiEngine.IGRestApiClient.createWorkingOrderV2(createWorkingOrderRequest).UseManagedCall();

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
                CreateWorkingOrderCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}