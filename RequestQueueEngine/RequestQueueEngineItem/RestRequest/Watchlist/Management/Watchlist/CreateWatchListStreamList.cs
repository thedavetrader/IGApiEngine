using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Configuration;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueItem
    {
        /// <summary>
        /// Create default watchlist if not found 
        /// </summary>
        /// <param name="iGApiDbContext"></param>
        public static void CreateWatchListStreamList(DbSet<Watchlist> Watchlists)
        {
            if (ConfigurationManager.GetSection("Watchlist") is NameValueCollection WatchlistConfig)
            {
                var streamListName = WatchlistConfig["EpicStreamList"];

                if (!Watchlists.Any(a => a.Name == streamListName))
                {
                    string? jsonParameters = null;
                    jsonParameters = JsonConvert.SerializeObject(new { name = streamListName, epics = ApiEngine.Instance.EpicStreamList.Select(s => s.Epic) }, Formatting.None);

                    RequestQueueItem.QueueItem(nameof(RequestQueueItem.CreateWatchlist), true, false, jsonParameters);
                    RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetWatchlists), true, false);
                }

                var defaultWatchList = Watchlists.FirstOrDefault(f =>
                    f.AccountId == ApiEngine.Instance.LoginSessionInformation.currentAccountId &&
                    f.Name == streamListName);

                if (defaultWatchList is not null)
                    RequestQueueItem.QueueItem(nameof(RequestQueueItem.GetWatchListEpics), true, false, defaultWatchList.Id);
            }
        }
    }
}