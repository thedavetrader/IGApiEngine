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
                using ApiDbContext apiDbContext = new();

                _ = apiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(apiDbContext.Accounts));

                var response = _apiEngine.IGRestApiClient.accountBalance().UseManagedCall();

                if (response is not null)
                {
                    response.Response.accounts.ForEach(account =>
                    {
                        if (account is not null)
                            apiDbContext.SaveAccount(account, account.balance);
                    });
                    Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
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