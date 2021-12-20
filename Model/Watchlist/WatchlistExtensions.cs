using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static Watchlist? SaveWatchlist(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.watchlists.retrieve.Watchlist watchlist,
            [NotNullAttribute] string accountId
            )
        {
            _ = iGApiDbContext.Watchlists ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Watchlists));

            var currentWatchlist = Task.Run(async () => await iGApiDbContext.Watchlists.FindAsync(accountId, watchlist.id)).Result;

            if (currentWatchlist is not null)
                currentWatchlist.MapProperties(watchlist, accountId);
            else
                currentWatchlist = iGApiDbContext.Watchlists.Add(new Watchlist(watchlist, accountId)).Entity;

            return currentWatchlist;
        }
    }
}