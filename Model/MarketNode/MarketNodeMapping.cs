using System.Diagnostics.CodeAnalysis;
using dto.endpoint.browse;
using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    public partial class MarketNode
    {
        public void MapProperties(
            [NotNullAttribute] HierarchyNode HierarchyNode,
            string? parentMarketNodeId,
            bool isBrowsed
            )
        {
            ApiLastUpdate = DateTime.UtcNow;
            MarketNodeId = HierarchyNode.id;
            Name = HierarchyNode.name;
            ParentMarketNodeId = parentMarketNodeId;
            IsBrowsed = isBrowsed;
        }
    }
}