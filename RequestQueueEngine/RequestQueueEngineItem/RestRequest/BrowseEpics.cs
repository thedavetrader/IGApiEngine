using IGApi.Model;
using IGApi.Common;
using Newtonsoft.Json;

namespace IGApi.RequestQueue
{
    using static Log;
    public partial class RequestQueueEngineItem
    {
        public static event EventHandler? BrowseEpicsCompleted;
        /*
         * table browse_market_node
         *  - string NodeId 
         *  - string ParentNodeId
         *  - string name
         *  - string Is browsed
         *  
         * On TASK/METHOD start:
         *  queitem: browsemarket(browse_market_node.where(isBrowed: false).firstordefault() ?? "0")    asap: no, isrecurrent: no   (also browse inifinity wehn all isBrowsed: true)
         *      - set This.Node isBrowsed: true
         *      - if node type: node save to browse_market_node, isBrsowed: false
         *      - if node type: market get_epic_details all markets in node
         *      - if (browse_market_node.where(isBrowed: false).Any)
         *      -   queitem: browsemarket(browse_market_node.where(isBrowed: false).firstordefault() ?? "0") asap: no, isrecurrent: no <- impicit recursive
         *      - else (rebrowse all nodes until infinity)
         *      -   update ALL browse_market_node set isBrowsed: false
         *      
         * */
        [RequestType(isRestRequest: true, isTradingRequest: false)]
        public void BrowseEpics()
        {
            try
            {
                using ApiDbContext apiDbContext = new();

                string marketNodeId;

                if (!apiDbContext.MarketNodes.Any(a => !a.IsBrowsed) && apiDbContext.MarketNodes.Any())
                    apiDbContext.MarketNodes.ToList().ForEach(a => a.IsBrowsed = false);

                var marketNode = apiDbContext.MarketNodes.Where(w => !w.IsBrowsed).FirstOrDefault();

                if (marketNode is not null)
                {
                    marketNodeId = marketNode.MarketNodeId;
                    marketNode.IsBrowsed = true;
                }
                else
                    marketNodeId = "0";

                var browseMarketsResponse = _apiEngine.IGRestApiClient.browse(marketNodeId).UseManagedCall().Response;

                if (browseMarketsResponse is not null)
                {
                    if (browseMarketsResponse.nodes is not null)
                    {
                        foreach (var node in browseMarketsResponse.nodes)
                            if (!string.IsNullOrEmpty(node.id))
                                apiDbContext.SaveMarketNode(node, marketNodeId, false);

                        apiDbContext.RemoveRange(apiDbContext.MarketNodes
                            .Where(w => w.ParentMarketNodeId == marketNodeId).ToList()
                            .Where(n => !browseMarketsResponse.nodes.Any(a => a.id == n.MarketNodeId)));
                    }
                    else if (browseMarketsResponse.markets is not null)
                    {
                        var markets = browseMarketsResponse.markets;
                        //we refresh all details
                        //var newEpics = markets.Where(w => !apiDbContext.EpicDetails.Any(a => a.Epic == w.epic));

                        //if (newEpics is not null && newEpics.Any())
                        {
                            var parameters = markets.Select(s => new { s.epic }).Distinct();

                            if (parameters.Any())
                            {
                                string? jsonParameters = null;

                                jsonParameters = JsonConvert.SerializeObject(
                                    parameters,
                                    Formatting.None);

                                RequestQueueEngineItem.QueueItem(nameof(RequestQueueEngineItem.GetEpicDetails), true, false, Guid.NewGuid(), null, jsonParameters, cancellationToken: _cancellationToken);
                            }
                        }
                    }
                }
                Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();  // Use wait to prevent the Task object is disposed while still saving the changes.
            }
            catch (Exception ex)
            {
                WriteException(ex);
                throw;
            }
            finally
            {
                QueueItemComplete(BrowseEpicsCompleted);
            }
        }
    }
}