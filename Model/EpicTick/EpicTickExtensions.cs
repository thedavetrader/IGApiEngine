using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        [Obsolete("Use natively compiled stored prcedure call \"EpicTick.SaveEpicTick\" for saving to EpicTick.", true)]
        public static EpicTick? SaveEpicTick(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] L1LsPriceData l1LsPriceData,
            [NotNullAttribute] string epic
            )
        {
            var currentEpicTick = Task.Run(async () => await apiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (currentEpicTick is not null)
                currentEpicTick.MapProperties(l1LsPriceData, epic);
            else
                currentEpicTick = apiDbContext.EpicTicks.Add(new EpicTick(l1LsPriceData, epic)).Entity;

            return currentEpicTick;
        }

        [Obsolete("Use natively compiled stored prcedure call \"EpicTick.SaveEpicTick\" for saving to EpicTick.", true)]
        public static EpicTick? SaveEpicTick(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.positions.get.otc.v2.MarketData marketData
            )
        {
            var epic = marketData.epic;

            var currentEpicTick = Task.Run(async () => await apiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (currentEpicTick is not null)
                currentEpicTick.MapProperties(marketData);
            else
                currentEpicTick = apiDbContext.EpicTicks.Add(new EpicTick(marketData)).Entity;

            return currentEpicTick;
        }

        [Obsolete("Use natively compiled stored prcedure call \"EpicTick.SaveEpicTick\" for saving to EpicTick.", true)]
        public static EpicTick? SaveEpicTick(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.workingorders.get.v2.MarketData marketData
            )
        {
            var epic = marketData.epic;

            var currentEpicTick = Task.Run(async () => await apiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (currentEpicTick is not null)
                currentEpicTick.MapProperties(marketData);
            else
                currentEpicTick = apiDbContext.EpicTicks.Add(new EpicTick(marketData)).Entity;

            return currentEpicTick;
        }

    }
}