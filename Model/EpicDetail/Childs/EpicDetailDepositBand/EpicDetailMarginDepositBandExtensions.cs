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
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] DepositBand DepositBand
            )
        {
            _ = iGApiDbContext.EpicDetailsMarginDepositBand ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsMarginDepositBand));

            var epicDetailDepositBand = Task.Run(async () => await iGApiDbContext.EpicDetailsMarginDepositBand.FindAsync(epicDetail.Epic, DepositBand.currency, DepositBand.min)).Result;

            if (epicDetailDepositBand is not null)
                epicDetailDepositBand.MapProperties(epicDetail, DepositBand);
            else
                epicDetailDepositBand = iGApiDbContext.EpicDetailsMarginDepositBand.Add(new EpicDetailMarginDepositBand(epicDetail, DepositBand)).Entity;

            return epicDetailDepositBand;
        }
    }
}