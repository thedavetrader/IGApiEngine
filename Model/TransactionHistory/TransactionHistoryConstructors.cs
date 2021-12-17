using System.Diagnostics.CodeAnalysis;
using dto.endpoint.accountactivity.transaction;
using IGApi.Common;

namespace IGApi.Model
{
    public partial class
TransactionHistory
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public
        TransactionHistory()
        {
            Currency = string.Format(Constants.InvalidEntry, nameof(TransactionHistory));
            InstrumentName = string.Format(Constants.InvalidEntry, nameof(TransactionHistory));
            TransactionType = string.Format(Constants.InvalidEntry, nameof(TransactionHistory));
            Reference = string.Format(Constants.InvalidEntry, nameof(TransactionHistory));
        }

        /// <summary>
        /// For creating new accounts using TransactionData
        /// </summary>
        /// <param name="TransactionData"></param>
        /// <param name="accountId"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public
        TransactionHistory(
            [NotNullAttribute] Transaction transaction
            )
        {
            MapProperties(transaction);

            _ = InstrumentName ?? throw new EssentialPropertyNullReferenceException(nameof(InstrumentName));
            _ = TransactionType ?? throw new EssentialPropertyNullReferenceException(nameof(TransactionType));
            _ = Reference ?? throw new EssentialPropertyNullReferenceException(nameof(Reference));
            _ = Currency ?? throw new EssentialPropertyNullReferenceException(nameof(Currency));
        }
    }
}
