using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using dto.endpoint.workingorders.get.v2;

namespace IGApi.Model
{
    public partial class WorkingOrder
    {
        public void MapProperties(
            [NotNullAttribute] WorkingOrderData WorkingOrderData,
            [NotNullAttribute] string accountId
            )
        {
            {
                AccountId = accountId;
                CreatedDate = WorkingOrderData.createdDate is null ?
                    CreatedDate :
                    DateTime.ParseExact(
                    WorkingOrderData.createdDate,
                    "yyyy/MM/dd HH:mm:ss:fff",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal);
                CurrencyCode = WorkingOrderData.currencyCode ?? CurrencyCode;
                DealId = WorkingOrderData.dealId ?? DealId;
                Direction = WorkingOrderData.direction ?? Direction;
                Dma = WorkingOrderData.dma;
                Epic = WorkingOrderData.epic ?? Epic;

                if (WorkingOrderData.goodTillDate is not null)
                {
                    DateTime goodTillDate;

                    if (
                        DateTime.TryParseExact(
                            WorkingOrderData.goodTillDate,
                            "yyyy/MM/dd HH:mm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal |
                            DateTimeStyles.AdjustToUniversal,
                            out goodTillDate))
                    { GoodTillDate = goodTillDate; }
                    else if (
                        DateTime.TryParseExact(
                            WorkingOrderData.goodTillDate,
                            "yyyy-MM-dd'T'HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal |
                            DateTimeStyles.AdjustToUniversal,
                            out goodTillDate))
                    { GoodTillDate = goodTillDate; }
                    else
                        throw new Exception($"Could not parse \"GoodTillDate\" value {WorkingOrderData.goodTillDate}.");
                }

                GuaranteedStop = WorkingOrderData.guaranteedStop;
                LimitDistance = WorkingOrderData.limitDistance;
                OrderLevel = WorkingOrderData.orderLevel;
                OrderSize = WorkingOrderData.orderSize;
                OrderType = WorkingOrderData.orderType ?? OrderType;
                StopDistance = WorkingOrderData.stopDistance;
                TimeInForce = WorkingOrderData.timeInForce ?? TimeInForce;
                ApiLastUpdate = DateTime.UtcNow;
            }
        }
    }
}