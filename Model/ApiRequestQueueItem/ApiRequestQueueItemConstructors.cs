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
        }

        public ApiRequestQueueItem(
            [NotNullAttribute] string restRequest,
            string? parameters,
            [NotNullAttribute] bool executeAsap,
            [NotNullAttribute] bool isRecurrent
        )
        {
            Request = restRequest;
            Parameters = parameters;
            ExecuteAsap = executeAsap;
            IsRecurrent = isRecurrent;

            if (executeAsap && isRecurrent)
                throw new Exception("A queueitem can not be recurrent as well a beeing executed as soon as possible.");
        }
    }
}