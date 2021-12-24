using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailMarginDepositBand? SaveEpicDetailMarginDepositBand(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] DepositBand DepositBand
            )
        {
            _ = apiDbContext.EpicDetailsMarginDepositBand ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetailsMarginDepositBand));

            var epicDetailDepositBand = Task.Run(async () => await apiDbContext.EpicDetailsMarginDepositBand.FindAsync(epicDetail.Epic, DepositBand.currency, DepositBand.min)).Result;

            if (epicDetailDepositBand is not null)
                epicDetailDepositBand.MapProperties(epicDetail, DepositBand);
            else
                epicDetailDepositBand = apiDbContext.EpicDetailsMarginDepositBand.Add(new EpicDetailMarginDepositBand(epicDetail, DepositBand)).Entity;

            return epicDetailDepositBand;
        }
    }
}