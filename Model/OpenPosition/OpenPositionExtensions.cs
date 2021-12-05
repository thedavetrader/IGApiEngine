using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static IGApiDbContext SaveOpenPosition(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.positions.get.otc.v2.OpenPositionData openPositionData,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string epic
            )
        {
            _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));

            var openPosition = Task.Run(async () => await iGApiDbContext.OpenPositions.FindAsync(accountId, openPositionData.dealId)).Result;

            if (openPosition is not null)
                openPosition.MapProperties(openPositionData, accountId, epic);
            else
                iGApiDbContext.OpenPositions.Add(new OpenPosition(openPositionData, accountId, epic));

            return iGApiDbContext;
        }

        public static IGApiDbContext SaveOpenPosition(
        [NotNullAttribute] this IGApiDbContext iGApiDbContext,
        [NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
        [NotNullAttribute] string accountId
        )
        {
            _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));

            var openPosition = Task.Run(async () => await iGApiDbContext.OpenPositions.FindAsync(accountId, lsTradeSubscriptionData.dealId)).Result;

            if (openPosition is not null)
                openPosition.MapProperties(lsTradeSubscriptionData, accountId);
            else
                iGApiDbContext.OpenPositions.Add(new OpenPosition(lsTradeSubscriptionData, accountId));

            return iGApiDbContext;
        }
    }
}