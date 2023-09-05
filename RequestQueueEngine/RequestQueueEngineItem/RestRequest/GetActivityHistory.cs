using IGApi.Common;
using IGApi.Model;
using IGWebApiClient;
using dto.endpoint.accountactivity.activity_v3;
using System.Globalization;

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
                string timePattern = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.SortableDateTimePattern;

                string fromDate;
                string toDate = DateTime.UtcNow.ToString(timePattern);   // Explicitly use local time, as IG rest call expects local time.

                using (ApiDbContext apiDbContext = new())
                {
                    if (apiDbContext.ActivitiesHistory.Any())
                        fromDate = TimeZoneInfo.ConvertTimeToUtc(
                            apiDbContext.ActivitiesHistory.Max(p => p.Timestamp)
                            , TimeZoneInfo.Local).ToString(timePattern);
                    else
                        fromDate = DateTime.MinValue.ToString(timePattern);
                }

                var response = _apiEngine.IGRestApiClient.lastActivityTimeRange_v3(fromDate, toDate).UseManagedCall();

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

            void SyncToDbActivitiesHistory(IgResponse<ActivityHistoryResponse_v3> response)
            {

                if (response.Response is not null)
                {
                    ApiDbContext apiDbContext = new();

                    response.Response.activities.ForEach(activity =>
                        {
                            if (activity is not null)
                                apiDbContext.SaveActivityHistory(activity);
                        });

                    Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
                }
            }
        }
    }
}