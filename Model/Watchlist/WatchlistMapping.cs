using System.Diagnostics.CodeAnalysis;

namespace IGApi.Model
{
    public partial class Watchlist
    {
        public void MapProperties(
            [NotNullAttribute] dto.endpoint.watchlists.retrieve.Watchlist watchlist,
            [NotNullAttribute] string accountId
            )
        {
            AccountId = accountId;
            DefaultSystemWatchlist = watchlist.defaultSystemWatchlist;
            Deleteable = watchlist.deleteable;
            Editable = watchlist.editable;
            Id = watchlist.id;
            Name = watchlist.name;
            ApiLastUpdate = DateTime.UtcNow;
        }
    }
}