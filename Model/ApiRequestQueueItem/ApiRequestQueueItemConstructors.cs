using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class ApiRequestQueueItem
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]      
        public ApiRequestQueueItem()
        {
            Request = string.Format(Constants.InvalidEntry, nameof(ApiRequestQueueItem));
        }

        public ApiRequestQueueItem(
            [NotNullAttribute] string restRequest,
            string? parameters,
            [NotNullAttribute] bool executeAsap,
            [NotNullAttribute] bool isRecurrent,
            Guid guid,
            Guid? parentGuid
        )
        {
            Request = restRequest;
            Timestamp = DateTime.UtcNow;
            Parameters = parameters;
            ExecuteAsap = executeAsap;
            IsRecurrent = isRecurrent;
            Guid = guid;
            ParentGuid = parentGuid;

            if (executeAsap && isRecurrent)
                throw new Exception("A queueitem can not be recurrent as well a beeing executed as soon as possible.");
        }
    }
}