using IGApi.Common;
using IGApi.IGApi.StreamingApi.StreamingTickData.EpicStreamListItem;
using Newtonsoft.Json.Linq;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? RemoveStreamListEpicCompleted;

        [RequestType(isRestRequest: false, isTradingRequest: false)]
        public void RemoveStreamListEpic()
        {
            try
            {
                var request = ApiRequestQueueItem.Parameters;

                if (request is not null)
                {
                    var jsonEpicArray = JArray.Parse(request).ToList();
                    
                    var removeEpics = jsonEpicArray
                        .Select(item =>
                        new EpicStreamListItem(
                            ((JObject)item).GetValue("epic", StringComparison.OrdinalIgnoreCase)?.Value<string>() ?? throw new ArgumentNullException(nameof(RemoveStreamListEpic)),
                            EpicStreamListItem.EpicStreamListItemSource.CustomTracked)
                        )
                        .ToList();

                    var parameters = _apiEngine.EpicStreamList.ToList();
                    parameters.RemoveAll(r => !removeEpics.Any(a => a.Epic == r.Epic));

                    _apiEngine.SyncEpicStreamListItems(parameters, EpicStreamListItem.EpicStreamListItemSource.CustomTracked);
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(RemoveStreamListEpicCompleted);
            }
        }
    }
}