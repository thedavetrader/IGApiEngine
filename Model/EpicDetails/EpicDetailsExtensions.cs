using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static IGApiDbContext SaveEpicDetail(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.marketdetails.v2.InstrumentData instrumentData
            )
        {
            _ = iGApiDbContext.EpicDetails ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.EpicDetails));

            var EpicDetail = Task.Run(async () => await iGApiDbContext.EpicDetails.FindAsync(instrumentData.epic)).Result;

            if (EpicDetail is not null)
                EpicDetail.MapProperties(instrumentData);
            else
                iGApiDbContext.EpicDetails.Add(new EpicDetail(instrumentData));

            return iGApiDbContext;
        }
    }
}