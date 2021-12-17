using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using dto.endpoint.accountactivity.transaction;

namespace IGApi.RestRequest
{
    public partial class ApiRequestItem
    {
        public void GetTransactionHistory()
        {
            try
            {
                string fromDate;
                string toDate = DateTime.Now.Date.ToString();   // Explicetly use local time, as IG rest call expects local time.

                using (IGApiDbContext iGApiDbContext = new())
                {
                    _ = iGApiDbContext.TransactionsHistory ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));
                    if (iGApiDbContext.TransactionsHistory.Any())
                        fromDate = iGApiDbContext.TransactionsHistory.Max(p => p.Date).Date.ToString();
                    else
                        fromDate = DateTime.MinValue.ToString();
                }

                var response = _apiEngine.IGRestApiClient.lastTransactionTimeRange("ALL", fromDate, toDate).UseManagedCall();

                SyncToDbTransactionsHistory(response);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(GetOpenPositions));
                throw;
            }

            void SyncToDbTransactionsHistory(IgResponse<TransactionHistoryResponse>? response)
            {
                IGApiDbContext iGApiDbContext = new();
                _ = iGApiDbContext.TransactionsHistory ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.TransactionsHistory));

                if (response is not null)
                {
                    response.Response.transactions.ForEach(transaction =>
                    {
                        if (transaction is not null)
                            iGApiDbContext.SaveTransactionHistory(transaction);
                    });

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}