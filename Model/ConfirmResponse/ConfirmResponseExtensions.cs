using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ConfirmResponse? SaveConfirmResponse(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] dto.endpoint.confirms.ConfirmsResponse confirmsResponse
            )
        {
            _ = iGApiDbContext.ConfirmResponses ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ConfirmResponses));

            var currentConfirmResponse = iGApiDbContext.ConfirmResponses.Add(new ConfirmResponse(confirmsResponse)).Entity;

            return currentConfirmResponse;
        }
    }
}