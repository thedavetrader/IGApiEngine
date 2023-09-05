using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.browse;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static MarketNode? SaveMarketNode(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] HierarchyNode HierarchyNode,
            string? parentMarketNodeId,
            bool isBrowsed
            )
        {
            var currentMarketNode = Task.Run(async () => await apiDbContext.MarketNodes.FindAsync(HierarchyNode.id)).Result;

            if (currentMarketNode is not null)
                currentMarketNode.MapProperties(HierarchyNode, parentMarketNodeId, isBrowsed);
            else
                currentMarketNode = apiDbContext.MarketNodes.Add(new MarketNode(HierarchyNode, parentMarketNodeId, isBrowsed)).Entity;

            return currentMarketNode;
        }
    }
}