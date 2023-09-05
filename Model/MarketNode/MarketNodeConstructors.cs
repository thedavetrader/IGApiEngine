using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.browse;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    public partial class MarketNode
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public MarketNode()
        {
            MarketNodeId = string.Format(Constants.InvalidEntry, nameof(MarketNode));
            Name = string.Format(Constants.InvalidEntry, nameof(MarketNode));
        }

        public MarketNode(
            [NotNullAttribute] HierarchyNode HierarchyNode,
            string? parentMarketNodeId,
            bool isBrowsed
            )
        {
            MapProperties(HierarchyNode, parentMarketNodeId, isBrowsed);

            _ = MarketNodeId ?? throw new PrimaryKeyNullReferenceException(nameof(MarketNodeId));
            _= Name ?? throw new EssentialPropertyNullReferenceException(nameof(Name));
        }
    }
}