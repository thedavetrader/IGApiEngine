using System.Globalization;
using IGApi.Common;
using IGApi.Model;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        private void checkConfirmationReceived(string dealReference)
        {
            const int timeout = 2 * 100;    //TODO: Make configurable
            bool isReceived = false;

            IGApiDbContext iGApiDbContext = new();
            _ = iGApiDbContext.ConfirmResponses ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ConfirmResponses));

            //  Wait until timeout
            for (int i = 0; i < timeout && !isReceived; i++)
            {
                isReceived = iGApiDbContext.ConfirmResponses.Any(c => c.DealReference == dealReference);
                Utility.WaitFor(10);
            }

            if (!isReceived)
            {
                WriteLog(Messages($"[{nameof(checkConfirmationReceived)}]", $"Warning, no confirmation received after {2} seconds. Using a new login and REST to retreive confirm and resubsribe to trade detail stream."));

                var response = _apiEngine.IGRestApiClient.retrieveConfirm(dealReference).UseManagedCall();

                if (response != null)
                {
                    iGApiDbContext.SaveConfirmResponse(response);

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

                    isReceived = true;
                }
                else
                    WriteException(new RestCallNullReferenceException());

                _apiEngine.Restart();
            }
        }
    }
}
