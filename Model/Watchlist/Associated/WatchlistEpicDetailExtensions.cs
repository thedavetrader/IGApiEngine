using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static WatchlistEpicDetail? SaveWatchlistEpicDetail(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string id,
            [NotNullAttribute] dto.endpoint.watchlists.retrieve.WatchlistMarket watchlistMarket
            )
        {
            return SaveWatchlistEpicDetail(apiDbContext, accountId, id, watchlistMarket.epic);
        }

        public static WatchlistEpicDetail? SaveWatchlistEpicDetail(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string id,
            [NotNullAttribute] string epic
            )
        {
            var currentWatchlistEpicDetail = Task.Run(async () => await apiDbContext.WatchlistEpicDetails.FindAsync(accountId, id, epic)).Result;

            if (currentWatchlistEpicDetail is null)
                currentWatchlistEpicDetail = apiDbContext.WatchlistEpicDetails.Add(new WatchlistEpicDetail(accountId: accountId, watchlistId: id, epic: epic)).Entity;

            return currentWatchlistEpicDetail;
        }
    }
}