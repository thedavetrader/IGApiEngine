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
            _ = apiDbContext.TransactionsHistory ?? throw new DBContextNullReferenceException(nameof(apiDbContext.TransactionsHistory));

            var currentTransaction = Task.Run(async () => await apiDbContext.TransactionsHistory.FindAsync(transaction.GetDate(), transaction.reference)).Result;

            if (currentTransaction is not null)
                currentTransaction.MapProperties(transaction);
            else
                currentTransaction = apiDbContext.TransactionsHistory.Add(new TransactionHistory(transaction)).Entity;

            return currentTransaction;
        }

        public static DateTime GetDate(this Transaction transaction)
        {

            if (
                DateTime.TryParseExact(
                transaction.date,
                "dd/MM/yy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out DateTime date))
            {
                return TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.Local).Date; // Use .Date to get the Midnight value <date> 00:00:00.
            }
            else
                throw new EssentialPropertyNullReferenceException(nameof(GetDate));
        }
    }
}