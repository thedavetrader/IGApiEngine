using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetailCurrency? SaveEpicDetailCurrency(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] EpicDetail epicDetail,
            [NotNullAttribute] CurrencyData currencyData
            )
        {
            #region SyncToDb Referencing entity
            var currency = apiDbContext.SaveCurrency(currencyData);
            #endregion

            EpicDetailCurrency? epicDetailCurrency = null;

            if (currency is not null)
            {
                epicDetailCurrency = Task.Run(async () => await apiDbContext.EpicDetailsCurrency.FindAsync(epicDetail.Epic, currencyData.code)).Result;

                if (epicDetailCurrency is not null)
                    epicDetailCurrency.MapProperties(epicDetail, currency);
                else
                    epicDetailCurrency = apiDbContext.EpicDetailsCurrency.Add(new EpicDetailCurrency(epicDetail, currency)).Entity;
            }

            return epicDetailCurrency;
        }
    }
}