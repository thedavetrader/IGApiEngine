using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class RestRequestQueueItem
    {
        public RestRequestQueueItem(
            [NotNullAttribute] string restRequest,
            string? parameters,
            [NotNullAttribute] bool executeAsap,
            [NotNullAttribute] bool isRecurrent
        )
        {
            RestRequest = restRequest;
            Parameters = parameters;
            ExecuteAsap = executeAsap;
            IsRecurrent = isRecurrent;

            if (executeAsap && isRecurrent)
                throw new Exception("A queueitem can not be recurrent as well a beeing executed as soon as possible.");
        }
    }
}