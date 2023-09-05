using IGApi.Model;
using IGApi.Common;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetAccountDetailsCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetAccountDetails()
        {
            try
            {

                var response = _apiEngine.IGRestApiClient.accountBalance().UseManagedCall();

                if (response.Response is not null)
                {
                    using ApiDbContext apiDbContext = new();

                    response.Response.accounts.ForEach(account =>
                        {
                            if (account is not null)
                                apiDbContext.SaveAccount(account, account.balance);
                        });
                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(GetAccountDetailsCompleted);
            }
        }
    }
}