using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static Watchlist? SaveWatchlist(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.watchlists.retrieve.Watchlist watchlist,
            [NotNullAttribute] string accountId
            )
        {
            var currentWatchlist = Task.Run(async () => await apiDbContext.Watchlists.FindAsync(accountId, watchlist.id)).Result;

            if (currentWatchlist is not null)
                currentWatchlist.MapProperties(watchlist, accountId);
            else
                currentWatchlist = apiDbContext.Watchlists.Add(new Watchlist(watchlist, accountId)).Entity;

            return currentWatchlist;
        }
    }
}