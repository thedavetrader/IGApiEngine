using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGApi;
using IGApi.Model;
using IGApiEngine.Common;

namespace IGApiEngine.IGRestApiQueueEngine
{
    public partial class RestRequest
    {
        private static void GetAccountDetails()
        {
            var response = Task.Run(async () => await IGApi.IGApiEngine.Instance.IGRestApiClient.accountBalance()).Result;

            _ = response ?? throw new RestCallNullException(nameof(response));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var accounts = response.Response.accounts;

                using IGApiDbContext iGApiDbContext = new();

                _ = iGApiDbContext.IGApiAccountDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.IGApiAccountDetails));

                accounts.ForEach(account =>
                {
                    var iGApiAccountDetails = iGApiDbContext.IGApiAccountDetails.SingleOrDefault(s => s.accountId == account.accountId);

                    if (iGApiAccountDetails is not null)
                        iGApiDbContext.Entry(iGApiAccountDetails).CurrentValues.SetValues(account);
                    else
                        iGApiDbContext.IGApiAccountDetails.Add(
                            new IGApiAccountDetails()
                            {
                                accountAlias        = account.accountAlias,
                                accountId           = account.accountId,
                                accountName         = account.accountName,
                                accountType         = account.accountType,
                                balance             = account.balance,  
                                canTransferFrom     = account.canTransferFrom,
                                canTransferTo       = account.canTransferTo,
                                currency            = account.currency,
                                preferred           = account.preferred,
                                status              = account.status
                            });
                });

                Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait(); // Use wait to prevent the task object is disposed while still saving the changes.
            }
            else
                throw new RestCallHttpRequestException(nameof(GetAccountDetails), response.StatusCode);
        }
    }
}