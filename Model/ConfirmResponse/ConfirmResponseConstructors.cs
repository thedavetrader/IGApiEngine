using System.Diagnostics.CodeAnalysis;
using dto.endpoint.accountactivity.transaction;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class ConfirmResponse
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public
        ConfirmResponse()
        {
            DealId = string.Format(Constants.InvalidEntry, nameof(ConfirmResponse));
            DealReference = string.Format(Constants.InvalidEntry, nameof(ConfirmResponse));
        }

        /// <summary>
        /// For creating new accounts using TransactionData
        /// </summary>
        /// <param name="TransactionData"></param>
        /// <param name="accountId"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public
        ConfirmResponse(
            [NotNullAttribute] dto.endpoint.confirms.ConfirmsResponse confirmsResponse
            )
        {
            MapProperties(confirmsResponse);

            _ = DealReference ?? throw new PrimaryKeyNullReferenceException(nameof(DealReference));
            _ = DealId ?? throw new EssentialPropertyNullReferenceException(nameof(DealId));
        }
    }
}
