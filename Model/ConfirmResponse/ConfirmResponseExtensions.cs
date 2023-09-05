using System.Diagnostics.CodeAnalysis;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static ConfirmResponse? SaveConfirmResponse(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] dto.endpoint.confirms.ConfirmsResponse confirmsResponse,
            [NotNullAttribute] bool isConsumable = false
            )
        {
            //  Cleanup any previous confirm responses for this dealreference (left overs from eg. crash)
            apiDbContext.ConfirmResponses.RemoveRange(apiDbContext.ConfirmResponses.Where(w => w.DealReference == confirmsResponse.dealReference));

            //  Confirm response are only added. They are deleted (consumed) by the calling db request procedure.
            var currentConfirmResponse = apiDbContext.ConfirmResponses.Add(new ConfirmResponse(confirmsResponse, isConsumable)).Entity;

            return currentConfirmResponse;
        }
    }
}