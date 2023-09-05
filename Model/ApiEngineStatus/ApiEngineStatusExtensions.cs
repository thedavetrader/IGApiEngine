using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using dto.endpoint.marketdetails.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ApiEngineStatus? SaveApiEngineStatus(
        [NotNullAttribute] this ApiDbContext apiDbContext,
        [NotNullAttribute] DateTime timestamp
        )
        {
            if (apiDbContext.ApiEngineState.Count() > 1)
                apiDbContext.ApiEngineState.RemoveRange(apiDbContext.ApiEngineState);

            var currentApiEngineStatus = Task.Run(() => apiDbContext.ApiEngineState.FirstOrDefault()).Result;

            if (currentApiEngineStatus is not null)
                currentApiEngineStatus.MapProperties(timestamp);
            else
                currentApiEngineStatus = apiDbContext.ApiEngineState.Add(new ApiEngineStatus(timestamp)).Entity;

            return currentApiEngineStatus;
        }
    }
}