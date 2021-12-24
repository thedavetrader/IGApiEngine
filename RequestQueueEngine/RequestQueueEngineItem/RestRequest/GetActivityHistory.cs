using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using dto.endpoint.accountactivity.activity;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? GetActivityHistoryCompleted;

        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void GetActivityHistory()
        {
            try
            {
                string fromDate;
                string toDate = DateTime.Now.Date.ToString();   // Explicetly use local time, as IG rest call expects local time.

                using (ApiDbContext apiDbContext = new())
                {
                    _ = apiDbContext.ActivitiesHistory ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetails));
                    if (apiDbContext.ActivitiesHistory.Any())
                        fromDate = apiDbContext.ActivitiesHistory.Max(p => p.Timestamp).Date.ToString();
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
                QueueItemComplete(GetActivityHistoryCompleted);
            }

            void SyncToDbActivitiesHistory(IgResponse<ActivityHistoryResponse>? response)
            {
                ApiDbContext apiDbContext = new();
                _ = apiDbContext.ActivitiesHistory ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ActivitiesHistory));

                if (response is not null)
                {
                    response.Response.activities.ForEach(activity =>
                    {
                        if (activity is not null)
                            apiDbContext.SaveActivityHistory(activity);
                    });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync()).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}