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
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] OpenPositionData openPositionData,
            [NotNullAttribute] string accountId,
            [NotNullAttribute] string epic
            )
        {
            _ = iGApiDbContext.OpenPositions ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.OpenPositions));

            var openPosition = Task.Run(async () => await iGApiDbContext.OpenPositions.FindAsync(accountId, openPositionData.dealId)).Result;

            if (openPosition is not null)
                openPosition.MapProperties(openPositionData, accountId, epic);
            else
                openPosition = iGApiDbContext.OpenPositions.Add(new OpenPosition(openPositionData, accountId, epic)).Entity;

            return openPosition;
        }

        public static OpenPosition? SaveOpenPosition(
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
                openPosition = iGApiDbContext.OpenPositions.Add(new OpenPosition(lsTradeSubscriptionData, accountId)).Entity;

            //TODO:     ZEROPRIO Example of ChangeTracker.DebugView Just for reference.
            iGApiDbContext.ChangeTracker.DetectChanges();
            Debug.WriteLine(iGApiDbContext.ChangeTracker.DebugView.LongView);

            return openPosition;
        }
    }
}