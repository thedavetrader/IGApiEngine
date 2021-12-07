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
                instrumentData.specialInfo.ForEach(SpecialInfo =>
                    iGApiDbContext.SaveEpicDetailSpecialInfo(epicDetail, SpecialInfo));

                instrumentData.currencies.ForEach(Currency =>
                    iGApiDbContext.SaveEpicDetailCurrency(epicDetail, Currency));

                instrumentData.marginDepositBands.ForEach(depositBand =>
                    iGApiDbContext.SaveEpicDetailMarginDepositBand(epicDetail, depositBand));

                if (instrumentData.openingHours is not null)
                    instrumentData.openingHours.marketTimes.ForEach(openingHours =>
                        iGApiDbContext.SaveEpicDetailOpeningHour(epicDetail, openingHours));
            }

            //TODO:     PRIOLOW Example of ChangeTracker.DebugView Just for reference.
            //iGApiDbContext.ChangeTracker.DetectChanges();
            //Debug.WriteLine(iGApiDbContext.ChangeTracker.DebugView.LongView);

            return epicDetail;
        }
    }
}