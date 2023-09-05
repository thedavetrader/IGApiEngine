using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using dto.endpoint.accountactivity.transaction;
using System.Globalization;

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
                string timePattern = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.SortableDateTimePattern;

                string fromDate;
                string toDate = DateTime.UtcNow.ToString(timePattern);   // Explicitly use local time, as IG rest call expects local time.

                using (ApiDbContext apiDbContext = new())
                {
                    if (apiDbContext.TransactionsHistory.Any())
                        fromDate = TimeZoneInfo.ConvertTimeToUtc(
                            apiDbContext.TransactionsHistory.Max(p => p.DateTime)
                            , TimeZoneInfo.Local).ToString(timePattern);
                    else
                        fromDate = DateTime.MinValue.ToString(timePattern);
                }

                var response = _apiEngine.IGRestApiClient.lastTransactionTimeRange_v2("ALL", fromDate, toDate).UseManagedCall();

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

            void SyncToDbTransactionsHistory(IgResponse<TransactionHistoryResponse> response)
            {
                if (response.Response is not null)
                {

                    ApiDbContext apiDbContext = new();

                    response.Response.transactions.ForEach(transaction =>
                    {
                        if (transaction is not null)
                            apiDbContext.SaveTransactionHistory(transaction);
                    });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}