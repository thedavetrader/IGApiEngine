﻿using System.Diagnostics.CodeAnalysis;
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

            //  Cleanup any previous confirm responses for this dealreference (left overs from eg. crash)
            iGApiDbContext.ConfirmResponses.RemoveRange(iGApiDbContext.ConfirmResponses.Where(w => w.DealReference == confirmsResponse.dealReference));

            //  Confirm response are only added. They are deleted (consumed) by the calling db request procedure.
            var currentConfirmResponse = iGApiDbContext.ConfirmResponses.Add(new ConfirmResponse(confirmsResponse)).Entity;

            return currentConfirmResponse;
        }
    }
}