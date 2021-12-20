using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ApiEngineStatus? SaveApiEngineStatus(
        [NotNullAttribute] this IGApiDbContext iGApiDbContext
        )
        {
            _ = iGApiDbContext.ApiEngineStatus ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ApiEngineStatus));

            if (iGApiDbContext.ApiEngineStatus.Count() > 1)
                iGApiDbContext.ApiEngineStatus.RemoveRange(iGApiDbContext.ApiEngineStatus);

            var currentApiEngineStatus = Task.Run(() => iGApiDbContext.ApiEngineStatus.FirstOrDefault()).Result;

            if (currentApiEngineStatus is not null)
                currentApiEngineStatus.MapProperties();
            else
                currentApiEngineStatus = iGApiDbContext.ApiEngineStatus.Add(new ApiEngineStatus()).Entity;

            return currentApiEngineStatus;
        }
    }
}