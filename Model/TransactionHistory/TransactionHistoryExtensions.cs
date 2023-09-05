using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IGApi.Common;
using dto.endpoint.accountactivity.transaction;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static TransactionHistory? SaveTransactionHistory(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] Transaction transaction
            )
        {
            var currentTransaction = Task.Run(async () => await apiDbContext.TransactionsHistory.FindAsync(transaction.GetDateTime(), transaction.reference)).Result;

            if (currentTransaction is not null)
                currentTransaction.MapProperties(transaction);
            else
                currentTransaction = apiDbContext.TransactionsHistory.Add(new TransactionHistory(transaction)).Entity;

            return currentTransaction;
        }

        public static DateTime GetDateTime(this Transaction transaction)
        {
            return DateTime.Parse(
                transaction.dateUtc,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal);
        }
    }
}