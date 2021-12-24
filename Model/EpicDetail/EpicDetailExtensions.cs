using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetail? SaveEpicDetail(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] InstrumentData instrumentData,
            DealingRulesData? dealingRulesData
            )
        {
            _ = apiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetails));

            var currentEpicDetail = Task.Run(async () => await apiDbContext.EpicDetails.FindAsync(instrumentData.epic)).Result;

            if (currentEpicDetail is not null)
                currentEpicDetail.MapProperties(instrumentData, dealingRulesData);
            else
                currentEpicDetail = apiDbContext.EpicDetails.Add(new EpicDetail(instrumentData, dealingRulesData)).Entity;

            // Save child and related entities.
            SaveSpecialInfo(apiDbContext, instrumentData, currentEpicDetail);
            SaveCurrency(apiDbContext, instrumentData, currentEpicDetail);
            SaveMarginDepositBand(apiDbContext, instrumentData, currentEpicDetail);
            SaveOpeningHour(apiDbContext, instrumentData, currentEpicDetail);

            return currentEpicDetail;

            static void SaveSpecialInfo(ApiDbContext apiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.specialInfo is not null)
                {
                    //  Remove obsolete
                    _ = apiDbContext.EpicDetailsSpecialInfo ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetailsSpecialInfo));

                    apiDbContext.EpicDetailsSpecialInfo.RemoveRange(
                        apiDbContext.EpicDetailsSpecialInfo
                            .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                            .Where(a => !instrumentData.specialInfo.Any(b => b == a.SpecialInfo)));

                    //  Upsert
                    instrumentData.specialInfo.ForEach(SpecialInfo =>
                        apiDbContext.SaveEpicDetailSpecialInfo(currentEpicDetail, SpecialInfo));
                }
            }

            static void SaveCurrency(ApiDbContext apiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.currencies is not null)
                {
                    //  Remove obsolete
                    _ = apiDbContext.EpicDetailsCurrency ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetailsCurrency));

                    apiDbContext.EpicDetailsCurrency.RemoveRange(
                        apiDbContext.EpicDetailsCurrency
                        .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !instrumentData.currencies.Any(b => b.code == a.Code)));

                    //  Upsert
                    instrumentData.currencies.ForEach(Currency =>
                        apiDbContext.SaveEpicDetailCurrency(currentEpicDetail, Currency));
                }
            }

            static void SaveMarginDepositBand(ApiDbContext apiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.marginDepositBands is not null)
                {
                    //  Remove obsolete
                    _ = apiDbContext.EpicDetailsMarginDepositBand ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetailsMarginDepositBand));

                    apiDbContext.EpicDetailsMarginDepositBand.RemoveRange(
                        apiDbContext.EpicDetailsMarginDepositBand
                        .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !instrumentData.marginDepositBands.Any(b => b.min == a.Min)));

                    //  Upsert
                    instrumentData.marginDepositBands.ForEach(depositBand =>
                        apiDbContext.SaveEpicDetailMarginDepositBand(currentEpicDetail, depositBand));
                }
            }

            static void SaveOpeningHour(ApiDbContext apiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.openingHours is not null)
                {
                    //  Remove obsolete
                    _ = apiDbContext.EpicDetailsOpeningHour ?? throw new DBContextNullReferenceException(nameof(apiDbContext.EpicDetailsOpeningHour));

                    apiDbContext.EpicDetailsOpeningHour.RemoveRange(
                        apiDbContext.EpicDetailsOpeningHour
                        .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !instrumentData.openingHours.marketTimes.Any(b => Utility.ConvertLocalTimeStringToUtcTimespan(b.openTime) == a.OpenTime)));

                    //  Upsert
                    instrumentData.openingHours.marketTimes.ForEach(openingHours =>
                        apiDbContext.SaveEpicDetailOpeningHour(currentEpicDetail, openingHours));
                }
            }
        }
    }
}