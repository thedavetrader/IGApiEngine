using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using dto.endpoint.marketdetails.v2;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static EpicDetail? SaveEpicDetail(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] InstrumentData instrumentData
            )
        {
            _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

            var epicDetail = Task.Run(async () => await iGApiDbContext.EpicDetails.FindAsync(instrumentData.epic)).Result;

            if (epicDetail is not null)
                epicDetail.MapProperties(instrumentData);
            else
                epicDetail = iGApiDbContext.EpicDetails.Add(new EpicDetail(instrumentData)).Entity;

            // Save childs
            if (epicDetail is not null)
            {
                // TODO: Remove obsolete Childs
                /*
                 * PSEUDO
                 * - Find all child entries on DB 
                 *  - which do not exist in instrumentdata.child
                 * - remove entry
                 * */

                #region SpecialInfo
                if (instrumentData.specialInfo is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsSpecialInfo ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsSpecialInfo));

                    iGApiDbContext.EpicDetailsSpecialInfo.RemoveRange(
                        iGApiDbContext.EpicDetailsSpecialInfo
                            .Where(w => w.Epic == epicDetail.Epic).ToList()
                            .Where(a => !instrumentData.specialInfo.Any(b => b == a.SpecialInfo)));

                    //  Upsert
                    instrumentData.specialInfo.ForEach(SpecialInfo =>
                        iGApiDbContext.SaveEpicDetailSpecialInfo(epicDetail, SpecialInfo));
                }
                #endregion

                #region Currency
                if (instrumentData.currencies is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsCurrency ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsCurrency));

                    iGApiDbContext.EpicDetailsCurrency.RemoveRange(
                        iGApiDbContext.EpicDetailsCurrency
                        .Where(w => w.Epic == epicDetail.Epic).ToList()
                        .Where(a => !instrumentData.currencies.Any(b => b.code == a.Code)));

                    //  Upsert
                    instrumentData.currencies.ForEach(Currency =>
                        iGApiDbContext.SaveEpicDetailCurrency(epicDetail, Currency));
                }
                #endregion

                #region MarginDepositBands
                if (instrumentData.marginDepositBands is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsMarginDepositBand ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsMarginDepositBand));

                    iGApiDbContext.EpicDetailsMarginDepositBand.RemoveRange(
                        iGApiDbContext.EpicDetailsMarginDepositBand
                        .Where(w => w.Epic == epicDetail.Epic).ToList()
                        .Where(a => !instrumentData.marginDepositBands.Any(b => b.min == a.Min)));

                    //  Upsert
                    instrumentData.marginDepositBands.ForEach(depositBand =>
                        iGApiDbContext.SaveEpicDetailMarginDepositBand(epicDetail, depositBand));
                }
                #endregion

                #region OpeningHours
                if (instrumentData.openingHours is not null)
                {
                    //  Remove obsolete
                    _ = iGApiDbContext.EpicDetailsOpeningHour ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetailsOpeningHour));

                    iGApiDbContext.EpicDetailsOpeningHour.RemoveRange(
                        iGApiDbContext.EpicDetailsOpeningHour
                        .Where(w => w.Epic == epicDetail.Epic).ToList()
                        .Where(a => !instrumentData.openingHours.marketTimes.Any(b => Utility.ConvertLocalTimeStringToUtcTimespan(b.openTime) == a.OpenTime)));

                    //  Upsert
                    instrumentData.openingHours.marketTimes.ForEach(openingHours =>
                        iGApiDbContext.SaveEpicDetailOpeningHour(epicDetail, openingHours));
                }
                #endregion
            }

            //TODO:     ZEROPRIO Example of ChangeTracker.DebugView Just for reference.
            //iGApiDbContext.ChangeTracker.DetectChanges();
            //Debug.WriteLine(iGApiDbContext.ChangeTracker.DebugView.LongView);

            return epicDetail;
        }
    }
}