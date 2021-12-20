using IGApi.Model;
using IGApi.Common;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? GetAccountDetailsCompleted;
        public void GetAccountDetails()
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

                var response = _apiEngine.IGRestApiClient.accountBalance().UseManagedCall();

                if (response is not null)
                {
                    response.Response.accounts.ForEach(account =>
                    {
                        if (account is not null)
                            iGApiDbContext.SaveAccount(account, account.balance);
                    });

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                GetAccountDetailsCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}