using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static Currency? SaveCurrency(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            var currentCurrency = Task.Run(async () => await apiDbContext.Currencies.FindAsync(currencyData.code)).Result;

            if (currentCurrency is not null)
                currentCurrency.MapProperties(currencyData);
            else
                currentCurrency = apiDbContext.Currencies.Add(new Currency(currencyData)).Entity;

            return currentCurrency;
        }
    }
}