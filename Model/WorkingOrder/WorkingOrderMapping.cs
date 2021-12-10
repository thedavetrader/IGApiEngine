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
                CreatedDate = DateTime.ParseExact(
                    WorkingOrderData.createdDate,
                    "yyyy/MM/dd HH:mm:ss:fff",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal |
                    DateTimeStyles.AdjustToUniversal);
                CurrencyCode = WorkingOrderData.currencyCode;
                DealId = WorkingOrderData.dealId;
                Direction = WorkingOrderData.direction;
                Dma = WorkingOrderData.dma;
                Epic = WorkingOrderData.epic;
                if (WorkingOrderData.goodTillDate is not null)
                    GoodTillDate = DateTime.ParseExact(
                        WorkingOrderData.goodTillDate,
                        "yyyy/MM/dd HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal |
                        DateTimeStyles.AdjustToUniversal);
                GuaranteedStop = WorkingOrderData.guaranteedStop;
                LimitDistance = WorkingOrderData.limitDistance;
                OrderLevel = WorkingOrderData.orderLevel;
                OrderSize = WorkingOrderData.orderSize;
                OrderType = WorkingOrderData.orderType;
                StopDistance = WorkingOrderData.stopDistance;
                TimeInForce = WorkingOrderData.timeInForce;
                ApiLastUpdate = DateTime.UtcNow;
            }
        }
    }
}