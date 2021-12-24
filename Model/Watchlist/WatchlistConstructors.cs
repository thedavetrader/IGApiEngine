using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class Watchlist
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public Watchlist()
        {
            AccountId = string.Format(Constants.InvalidEntry, nameof(Watchlist));
            WatchlistId = string.Format(Constants.InvalidEntry, nameof(Watchlist));
            Name = string.Format(Constants.InvalidEntry, nameof(Watchlist));
        }

        /// <summary>
        /// For creating new accounts using WatchlistData
        /// </summary>
        /// <param name="WatchlistData"></param>
        /// <param name="accountId"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public Watchlist(
            [NotNullAttribute] dto.endpoint.watchlists.retrieve.Watchlist watchlist,
            [NotNullAttribute] string accountId
            )
        {
            MapProperties(watchlist, accountId);

            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
            _ = WatchlistId ?? throw new PrimaryKeyNullReferenceException(nameof(WatchlistId));
            _ = Name ?? throw new EssentialPropertyNullReferenceException(nameof(Name));
        }
       
    }
}
