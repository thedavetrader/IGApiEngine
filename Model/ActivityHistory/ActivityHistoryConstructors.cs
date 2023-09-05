using dto.endpoint.accountactivity.activity_v3;
using IGApi.Common;
using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class
ActivityHistory
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public
        ActivityHistory()
        {
            DealId = string.Format(Constants.InvalidEntry, nameof(Activity_v3));
            Status = string.Format(Constants.InvalidEntry, nameof(Activity_v3));
            Channel = string.Format(Constants.InvalidEntry, nameof(Activity_v3));
            Epic = string.Format(Constants.InvalidEntry, nameof(Activity_v3));
            Type = string.Format(Constants.InvalidEntry, nameof(Activity_v3));
            Description = string.Format(Constants.InvalidEntry, nameof(Activity_v3));
        }

        /// <summary>
        /// For creating new accounts using ActivityData
        /// </summary>
        /// <param name="ActivityData"></param>
        /// <param name="accountId"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public ActivityHistory(
            [NotNullAttribute] Activity_v3 activity
            )
        {
            MapProperties(activity);

            _ = DealId ?? throw new PrimaryKeyNullReferenceException(nameof(DealId));
            _ = Status ?? throw new EssentialPropertyNullReferenceException(nameof(Status));
            _ = Channel ?? throw new EssentialPropertyNullReferenceException(nameof(Channel));
            _ = Epic ?? throw new EssentialPropertyNullReferenceException(nameof(Epic));
            _ = Type ?? throw new EssentialPropertyNullReferenceException(nameof(Type));
            _ = Description ?? throw new EssentialPropertyNullReferenceException(nameof(Description));
        }
    }
}
