using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using dto.endpoint.workingorders.get.v2;

namespace IGApi.Model
{
    public partial class ApiRequestQueueItem
    {
        public void MapProperties(
            [NotNullAttribute] ApiRequestQueueItem restRequestQueueItem
            )
        {
            {
                ExecuteAsap = restRequestQueueItem.ExecuteAsap;
                IsRecurrent = restRequestQueueItem.IsRecurrent;
                Parameters = restRequestQueueItem.Parameters;
                Request = restRequestQueueItem.Request;
                Timestamp = restRequestQueueItem.Timestamp;
                IsRunning = restRequestQueueItem.IsRunning;
                Guid = restRequestQueueItem.Guid;
                ParentGuid = restRequestQueueItem.ParentGuid;
            }
        }
    }
}
