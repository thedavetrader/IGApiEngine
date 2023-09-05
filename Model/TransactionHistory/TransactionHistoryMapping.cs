using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IGApi.Common;
using dto.endpoint.accountactivity.transaction;

namespace IGApi.Model
{
    public partial class

TransactionHistory
    {
        public void MapProperties(
            [NotNullAttribute] Transaction transaction
            )
        {

            DateTime = transaction.GetDateTime();
            Reference = transaction.reference;
            TransactionType = transaction.transactionType;
            CashTransaction = transaction.cashTransaction;
            Currency = transaction.currency;
            InstrumentName = transaction.instrumentName;
            Period = transaction.period;

            if (decimal.TryParse(transaction.size, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal size))
                Size = size;
            if (decimal.TryParse(transaction.openLevel, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal openLevel))
                OpenLevel = openLevel;
            if (decimal.TryParse(transaction.closeLevel, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal closeLevel))
                CloseLevel = closeLevel;
            if (decimal.TryParse(transaction.profitAndLoss.Replace(transaction.currency, String.Empty), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal profitAndLoss))
                ProfitAndLoss = profitAndLoss;
        }
    }
}