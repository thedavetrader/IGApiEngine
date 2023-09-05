using System.Globalization;
using IGApi.Common;
using IGApi.Model;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        private void checkConfirmationReceived(string dealReference)
        {
            const int timeout = 2 * 100;    //TODO: Make configurable timout wait for confirmationreceived 
            bool isReceived = false;

            ApiDbContext apiDbContext = new();

            //  Wait until timeout
            for (int i = 0; i < timeout && !isReceived; i++)
            {
                var confirmResponse = apiDbContext.ConfirmResponses.FirstOrDefault(c => c.DealReference == dealReference);

                if (confirmResponse is not null)
                {
                    confirmResponse.IsConsumable = true;
                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                    isReceived = true;
                }
                else
                {
                    isReceived = false;
                    Utility.WaitFor(10);
                }
            }

            if (!isReceived)
            {
                WriteError(Columns($"[{nameof(checkConfirmationReceived)}]", $"[WARNING] no confirmation received after {2} seconds. Using REST to retreive confirm."));

                var response = _apiEngine.IGRestApiClient.retrieveConfirm(dealReference).UseManagedCall();

                if (response != null)
                {
                    apiDbContext.SaveConfirmResponse(response, isConsumable: true);

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.

                    isReceived = true;
                }
                else
                    WriteException(new RestCallNullReferenceException());
            }
        }
    }
}
