using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using dto.endpoint.accountactivity.activity;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        public static event EventHandler? GetActivityHistoryCompleted;
        public void GetActivityHistory()
        {
            try
            {
                string fromDate;
                string toDate = DateTime.Now.Date.ToString();   // Explicetly use local time, as IG rest call expects local time.

                using (IGApiDbContext iGApiDbContext = new())
                {
                    _ = iGApiDbContext.ActivitiesHistory ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));
                    if (iGApiDbContext.ActivitiesHistory.Any())
                        fromDate = iGApiDbContext.ActivitiesHistory.Max(p => p.Timestamp).Date.ToString();
                    else
                        fromDate = DateTime.MinValue.ToString();
                }

                var response = _apiEngine.IGRestApiClient.lastActivityTimeRange(fromDate, toDate).UseManagedCall();

                SyncToDbActivitiesHistory(response);
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                GetActivityHistoryCompleted?.Invoke(this, EventArgs.Empty);
            }

            void SyncToDbActivitiesHistory(IgResponse<ActivityHistoryResponse>? response)
            {
                IGApiDbContext iGApiDbContext = new();
                _ = iGApiDbContext.ActivitiesHistory ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ActivitiesHistory));

                if (response is not null)
                {
                    response.Response.activities.ForEach(activity =>
                    {
                        if (activity is not null)
                            iGApiDbContext.SaveActivityHistory(activity);
                    });

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}