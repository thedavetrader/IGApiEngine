using IGApi.Model;
using IGApi.Common;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        public void GetAccountDetails()
        {
            try
            {
                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

                var response = RestApiClientCall(_apiEngine.IGRestApiClient.accountBalance());

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
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }
        }
    }
}