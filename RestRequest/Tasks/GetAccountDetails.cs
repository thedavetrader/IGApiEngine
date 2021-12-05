using IGApi.Model;
using IGApi.Common;

namespace IGApi.RestRequest
{
    public partial class RestRequest
    {
        private void GetAccountDetails()
        {
            using IGApiDbContext iGApiDbContext = new();

            _ = iGApiDbContext.Accounts ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Accounts));

            #region RestRequestCall
            var response = Task.Run(async () => await _iGApiEngine.IGRestApiClient.accountBalance()).Result;

            _ = response ?? throw new RestCallNullReferenceException(nameof(response));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new RestCallHttpRequestException(nameof(GetAccountDetails), response.StatusCode);
            #endregion RestRequestCall

            response.Response.accounts.ForEach(account =>
            {
                if (account is not null)
                {
                    iGApiDbContext.SaveAccount(account, account.balance);
                }
            });

            Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
        }
    }
}