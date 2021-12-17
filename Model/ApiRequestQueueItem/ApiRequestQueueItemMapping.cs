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
                RestRequest = restRequestQueueItem.RestRequest;
                Timestamp = restRequestQueueItem.Timestamp; // Might be that in future Timestamp overwrite should be optional.
            }
        }
    }
}
