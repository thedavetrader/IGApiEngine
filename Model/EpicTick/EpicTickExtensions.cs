using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicTick? SaveEpicTick(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] L1LsPriceData l1LsPriceData,
            [NotNullAttribute] string epic
            )
        {
            _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

            var epicTick = Task.Run(async () => await iGApiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (epicTick is not null)
                epicTick.MapProperties(l1LsPriceData, epic);
            else
                epicTick = iGApiDbContext.EpicTicks.Add(new EpicTick(l1LsPriceData, epic)).Entity;

            return epicTick;
        }
        
        public static EpicTick? SaveEpicTick(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.positions.get.otc.v2.MarketData marketData
            )
        {
            _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

            var epic = marketData.epic;

            var epicTick = Task.Run(async () => await iGApiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (epicTick is not null)
                epicTick.MapProperties(marketData);
            else
                epicTick = iGApiDbContext.EpicTicks.Add(new EpicTick(marketData)).Entity;

            return epicTick;
        }        

        public static EpicTick? SaveEpicTick(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.workingorders.get.v2.MarketData marketData
            )
        {
            _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

            var epic = marketData.epic;

            var epicTick = Task.Run(async () => await iGApiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (epicTick is not null)
                epicTick.MapProperties(marketData);
            else
                epicTick = iGApiDbContext.EpicTicks.Add(new EpicTick(marketData)).Entity;

            return epicTick;
        }

    }
}