using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static Currency? SaveCurrency(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            _ = iGApiDbContext.Currencies ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.Currencies));

            var currentCurrency = Task.Run(async () => await iGApiDbContext.Currencies.FindAsync(currencyData.code)).Result;

            if (currentCurrency is not null)
                currentCurrency.MapProperties(currencyData);
            else
                currentCurrency = iGApiDbContext.Currencies.Add(new Currency(currencyData)).Entity;

            return currentCurrency;
        }
    }
}