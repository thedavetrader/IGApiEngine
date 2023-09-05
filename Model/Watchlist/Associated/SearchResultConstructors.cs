using System.Diagnostics.CodeAnalysis;
using dto.endpoint.accountactivity.transaction;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class WatchlistEpicDetail
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public
        WatchlistEpicDetail()
        {
            Epic = string.Format(Constants.InvalidEntry, nameof(WatchlistEpicDetail));
            AccountId = string.Format(Constants.InvalidEntry, nameof(WatchlistEpicDetail));
            WatchlistId = string.Format(Constants.InvalidEntry, nameof(WatchlistEpicDetail));
        }

        /// <summary>
        /// For creating new accounts using TransactionData
        /// </summary>
        /// <param name="WatchlistEpicDetail"></param>
        /// <param name="epic"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public
        WatchlistEpicDetail(
            [NotNullAttribute] string epic,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string watchlistId
            )
        {
            Epic = epic;
            AccountId = accountId;
            WatchlistId = watchlistId;
        }
    }
}
