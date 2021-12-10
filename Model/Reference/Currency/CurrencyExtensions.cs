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

            var Currency = Task.Run(async () => await iGApiDbContext.Currencies.FindAsync(currencyData.code)).Result;

            if (Currency is not null)
                Currency.MapProperties(currencyData);
            else
                Currency = iGApiDbContext.Currencies.Add(new Currency(currencyData)).Entity;

            return Currency;
        }
    }
}