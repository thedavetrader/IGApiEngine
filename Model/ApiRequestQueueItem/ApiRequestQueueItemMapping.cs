using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class ApiRequestQueueItem
    {
        public void MapProperties(
            [NotNullAttribute] ApiRequestQueueItem restRequestQueueItem
            )
        {
            ExecuteAsap = restRequestQueueItem.ExecuteAsap;
            IsRecurrent = restRequestQueueItem.IsRecurrent;
            Parameters = restRequestQueueItem.Parameters;
            Request = restRequestQueueItem.Request;
            Timestamp = restRequestQueueItem.Timestamp;
            Guid = restRequestQueueItem.Guid;
            ParentGuid = restRequestQueueItem.ParentGuid;
        }
    }
}
