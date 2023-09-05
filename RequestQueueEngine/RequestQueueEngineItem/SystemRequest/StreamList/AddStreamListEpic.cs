using IGApi.Common;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using Newtonsoft.Json.Linq;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? AddStreamListEpicCompleted;

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void AddStreamListEpic()
        {
            try
            {
                var request = ApiRequestQueueItem.Parameters;

                if (request is not null)
                {
                    var jsonEpicArray = JArray.Parse(request).ToList();
                    var parameters = jsonEpicArray
                        .Select(item =>
                        new EpicStreamListItem(
                            ((JObject)item).GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>() ?? throw new ArgumentNullException(nameof(AddStreamListEpic)),
                            EpicStreamListItem.EpicStreamListItemSource.CustomTracked)
                        )
                        .ToList();

                    parameters.AddRange(_apiEngine.EpicStreamList.Where(w => w.IsSource(EpicStreamListItem.EpicStreamListItemSource.CustomTracked)).Distinct());

                    _apiEngine.SyncEpicStreamListItems(parameters.ToList(), EpicStreamListItem.EpicStreamListItemSource.CustomTracked);
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(AddStreamListEpicCompleted);
            }
        }
    }
}