using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailCurrency? SaveEpicDetailCurrency(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            _ = iGApiDbContext.EpicDetailsCurrency ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsCurrency));

            var epicDetailCurrency = Task.Run(async () => await iGApiDbContext.EpicDetailsCurrency.FindAsync(epicDetail.Epic, currencyData.code)).Result;

            if (epicDetailCurrency is not null)
                epicDetailCurrency.MapProperties(epicDetail, currencyData);
            else
                epicDetailCurrency = iGApiDbContext.EpicDetailsCurrency.Add(new EpicDetailCurrency(epicDetail, currencyData)).Entity;

            return epicDetailCurrency;
        }
    }
}