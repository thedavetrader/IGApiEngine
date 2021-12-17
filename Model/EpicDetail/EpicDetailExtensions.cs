using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetail? SaveEpicDetail(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] InstrumentData instrumentData,
            DealingRulesData? dealingRulesData
            )
        {
            _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

            var currentEpicDetail = Task.Run(async () => await iGApiDbContext.EpicDetails.FindAsync(instrumentData.epic)).Result;

            if (currentEpicDetail is not null)
                currentEpicDetail.MapProperties(instrumentData, dealingRulesData);
            else
                currentEpicDetail = iGApiDbContext.EpicDetails.Add(new EpicDetail(instrumentData, dealingRulesData)).Entity;

            // Save child and related entities.
            SaveSpecialInfo(iGApiDbContext, instrumentData, currentEpicDetail);
            SaveCurrency(iGApiDbContext, instrumentData, currentEpicDetail);
            SaveMarginDepositBand(iGApiDbContext, instrumentData, currentEpicDetail);
            SaveOpeningHour(iGApiDbContext, instrumentData, currentEpicDetail);

            return currentEpicDetail;

            static void SaveSpecialInfo(IGApiDbContext iGApiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.specialInfo is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsSpecialInfo ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsSpecialInfo));

                    iGApiDbContext.EpicDetailsSpecialInfo.RemoveRange(
                        iGApiDbContext.EpicDetailsSpecialInfo
                            .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                            .Where(a => !instrumentData.specialInfo.Any(b => b == a.SpecialInfo)));

                    //  Upsert
                    instrumentData.specialInfo.ForEach(SpecialInfo =>
                        iGApiDbContext.SaveEpicDetailSpecialInfo(currentEpicDetail, SpecialInfo));
                }
            }

            static void SaveCurrency(IGApiDbContext iGApiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.currencies is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsCurrency ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsCurrency));

                    iGApiDbContext.EpicDetailsCurrency.RemoveRange(
                        iGApiDbContext.EpicDetailsCurrency
                        .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !instrumentData.currencies.Any(b => b.code == a.Code)));

                    //  Upsert
                    instrumentData.currencies.ForEach(Currency =>
                        iGApiDbContext.SaveEpicDetailCurrency(currentEpicDetail, Currency));
                }
            }

            static void SaveMarginDepositBand(IGApiDbContext iGApiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.marginDepositBands is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsMarginDepositBand ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsMarginDepositBand));

                    iGApiDbContext.EpicDetailsMarginDepositBand.RemoveRange(
                        iGApiDbContext.EpicDetailsMarginDepositBand
                        .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !instrumentData.marginDepositBands.Any(b => b.min == a.Min)));

                    //  Upsert
                    instrumentData.marginDepositBands.ForEach(depositBand =>
                        iGApiDbContext.SaveEpicDetailMarginDepositBand(currentEpicDetail, depositBand));
                }
            }

            static void SaveOpeningHour(IGApiDbContext iGApiDbContext, InstrumentData instrumentData, EpicDetail? currentEpicDetail)
            {
                if (currentEpicDetail is not null && instrumentData.openingHours is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsOpeningHour ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsOpeningHour));

                    iGApiDbContext.EpicDetailsOpeningHour.RemoveRange(
                        iGApiDbContext.EpicDetailsOpeningHour
                        .Where(w => w.Epic == currentEpicDetail.Epic).ToList() // Use ToList() to prevent that Linq constructs a predicate that can not be sent to db.
                        .Where(a => !instrumentData.openingHours.marketTimes.Any(b => Utility.ConvertLocalTimeStringToUtcTimespan(b.openTime) == a.OpenTime)));

                    //  Upsert
                    instrumentData.openingHours.marketTimes.ForEach(openingHours =>
                        iGApiDbContext.SaveEpicDetailOpeningHour(currentEpicDetail, openingHours));
                }
            }
        }
    }
}