using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ApiEngineStatus? SaveApiEngineStatus(
        [NotNullAttribute] this ApiDbContext apiDbContext
        )
        {
            _ = apiDbContext.ApiEngineStatus ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiEngineStatus));

            if (apiDbContext.ApiEngineStatus.Count() > 1)
                apiDbContext.ApiEngineStatus.RemoveRange(apiDbContext.ApiEngineStatus);

            var currentApiEngineStatus = Task.Run(() => apiDbContext.ApiEngineStatus.FirstOrDefault()).Result;

            if (currentApiEngineStatus is not null)
                currentApiEngineStatus.MapProperties();
            else
                currentApiEngineStatus = apiDbContext.ApiEngineStatus.Add(new ApiEngineStatus()).Entity;

            return currentApiEngineStatus;
        }
    }
}