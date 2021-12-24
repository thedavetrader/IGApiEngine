using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using dto.endpoint.accountactivity.transaction;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetTransactionHistoryCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetTransactionHistory()
        {
            try
            {
                string fromDate;
                string toDate = DateTime.Now.Date.ToString();   // Explicetly use local time, as IG rest call expects local time.

                using (ApiDbContext apiDbContext = new())
                {
                    _ = apiDbContext.TransactionsHistory ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetails));
                    if (apiDbContext.TransactionsHistory.Any())
                        fromDate = apiDbContext.TransactionsHistory.Max(p => p.Date).Date.ToString();
                    else
                        fromDate = DateTime.MinValue.ToString();
                }

                var response = _apiEngine.IGRestApiClient.lastTransactionTimeRange("ALL", fromDate, toDate).UseManagedCall();

                SyncToDbTransactionsHistory(response);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(GetTransactionHistoryCompleted);
            }

            void SyncToDbTransactionsHistory(IgResponse<TransactionHistoryResponse>? response)
            {
                ApiDbContext apiDbContext = new();
                _ = apiDbContext.TransactionsHistory ?? throw new DBContextNullReferenceException(nameof(apiDbContext.TransactionsHistory));

                if (response is not null)
                {
                    response.Response.transactions.ForEach(transaction =>
                    {
                        if (transaction is not null)
                            apiDbContext.SaveTransactionHistory(transaction);
                    });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}