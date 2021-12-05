using System.Diagnostics.CodeAnalysis;
using dto.endpoint.positions.get.otc.v2;
using IGApi.Common;
using IGWebApiClient;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static IGApiDbContext SaveEpicTick(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] L1LsPriceData l1LsPriceData,
            [NotNullAttribute] string epic
            )
        {
            _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

            var EpicTick = Task.Run(async () => await iGApiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (EpicTick is not null)
                EpicTick.MapProperties(l1LsPriceData, epic);
            else
                iGApiDbContext.EpicTicks.Add(new EpicTick(l1LsPriceData, epic));

            return iGApiDbContext;
        }
        
        public static IGApiDbContext SaveEpicTick(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] MarketData marketData
            )
        {
            _ = iGApiDbContext.EpicTicks ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicTicks));

            var epic = marketData.epic;

            var Tick = Task.Run(async () => await iGApiDbContext.EpicTicks.FindAsync(epic)).Result;

            if (Tick is not null)
                Tick.MapProperties(marketData);
            else
                iGApiDbContext.EpicTicks.Add(new EpicTick(marketData));

            return iGApiDbContext;
        }


    }
}