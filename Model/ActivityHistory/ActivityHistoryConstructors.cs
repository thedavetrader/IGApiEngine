using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;
using dto.endpoint.accountactivity.activity;

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
            DealId = string.Format(Constants.InvalidEntry, nameof(Activity));
            ActionStatus = string.Format(Constants.InvalidEntry, nameof(Activity));
            Activity = string.Format(Constants.InvalidEntry, nameof(Activity));
            ActivityHistoryId = string.Format(Constants.InvalidEntry, nameof(Activity));
            Channel = string.Format(Constants.InvalidEntry, nameof(Activity));
            Currency = string.Format(Constants.InvalidEntry, nameof(Activity));
            Epic = string.Format(Constants.InvalidEntry, nameof(Activity));
            MarketName = string.Format(Constants.InvalidEntry, nameof(Activity));
            Result = string.Format(Constants.InvalidEntry, nameof(Activity));
            Size = string.Format(Constants.InvalidEntry, nameof(Activity));
            Reference = string.Format(Constants.InvalidEntry, nameof(Activity));
        }

        /// <summary>
        /// For creating new accounts using ActivityData
        /// </summary>
        /// <param name="ActivityData"></param>
        /// <param name="accountId"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public
        ActivityHistory(
            [NotNullAttribute] Activity activity
            )
        {
            MapProperties(activity);

            _ = DealId ?? throw new PrimaryKeyNullReferenceException(nameof(DealId));
            _ = ActionStatus ?? throw new EssentialPropertyNullReferenceException(nameof(ActionStatus));
            _ = Activity ?? throw new EssentialPropertyNullReferenceException(nameof(Activity));
            _ = ActivityHistoryId ?? throw new EssentialPropertyNullReferenceException(nameof(ActivityHistoryId));
            _ = Channel ?? throw new EssentialPropertyNullReferenceException(nameof(Channel));
            _ = Currency ?? throw new EssentialPropertyNullReferenceException(nameof(Currency));
            _ = Epic ?? throw new EssentialPropertyNullReferenceException(nameof(Epic));
            _ = MarketName ?? throw new EssentialPropertyNullReferenceException(nameof(MarketName));
            _ = Result ?? throw new EssentialPropertyNullReferenceException(nameof(Result));
            _ = Size ?? throw new EssentialPropertyNullReferenceException(nameof(Size));
        }
    }
}
