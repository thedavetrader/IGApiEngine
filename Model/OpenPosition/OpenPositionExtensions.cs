using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;
using dto.endpoint.positions.get.otc.v2;
using System.Diagnostics;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static OpenPosition? SaveOpenPosition(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] OpenPositionData openPositionData,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string epic
            )
        {
            var currentOpenPosition = Task.Run(async () => await apiDbContext.OpenPositions.FindAsync(accountId, openPositionData.dealId)).Result;

            if (currentOpenPosition is not null)
                currentOpenPosition.MapProperties(openPositionData, accountId, epic);
            else
                currentOpenPosition = apiDbContext.OpenPositions.Add(new OpenPosition(openPositionData, accountId, epic)).Entity;

            return currentOpenPosition;
        }

        public static OpenPosition? SaveOpenPosition(
        [NotNullAttribute] this ApiDbContext apiDbContext,
        [NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
        [NotNullAttribute] string accountId
        )
        {
            var currentOpenPosition = Task.Run(async () => await apiDbContext.OpenPositions.FindAsync(accountId, lsTradeSubscriptionData.dealId)).Result;

            if (currentOpenPosition is not null)
                currentOpenPosition.MapProperties(lsTradeSubscriptionData, accountId);
            else
                currentOpenPosition = apiDbContext.OpenPositions.Add(new OpenPosition(lsTradeSubscriptionData, accountId)).Entity;

            //TODO:     ZEROPRIO Example of ChangeTracker.DebugView Just for reference.
            //apiDbContext.ChangeTracker.DetectChanges();
            //Debug.WriteLine(apiDbContext.ChangeTracker.DebugView.LongView);

            return currentOpenPosition;
        }
    }
}