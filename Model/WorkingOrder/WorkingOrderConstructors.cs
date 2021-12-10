using System.Diagnostics.CodeAnalysis;
using IGApi.Common;
using IGWebApiClient;
using dto.endpoint.workingorders.get.v2;

namespace IGApi.Model
{
    public partial class WorkingOrder
    {
        /// <summary>
        /// Also provide normal constructor for EF-Core.
        /// </summary>
        [Obsolete("Do not use this constructor. It's intended use is for EF-Core only.", true)]
        public WorkingOrder()
        {
            AccountId = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
            DealId = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
            Epic = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
            Direction = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
            TimeInForce = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
            OrderType = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
            CurrencyCode = string.Format(Constants.InvalidEntry, nameof(WorkingOrder));
        }

        /// <summary>
        /// For creating new accounts using WorkingOrderData
        /// </summary>
        /// <param name="WorkingOrderData"></param>
        /// <param name="accountId"></param>
        /// <exception cref="PrimaryKeyNullReferenceException"></exception>
        /// <exception cref="EssentialPropertyNullReferenceException"></exception>
        public WorkingOrder(
            [NotNullAttribute] WorkingOrderData WorkingOrderData,
            [NotNullAttribute] string accountId
            )
        {
            MapProperties(WorkingOrderData, accountId);

            _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
            _ = DealId ?? throw new EssentialPropertyNullReferenceException(nameof(DealId));
            _ = Epic ?? throw new EssentialPropertyNullReferenceException(nameof(Epic));
            _ = Direction ?? throw new EssentialPropertyNullReferenceException(nameof(Direction));
            _ = TimeInForce ?? throw new EssentialPropertyNullReferenceException(nameof(TimeInForce));
            _ = OrderType ?? throw new EssentialPropertyNullReferenceException(nameof(OrderType));
            _ = CurrencyCode ?? throw new EssentialPropertyNullReferenceException(nameof(CurrencyCode));
        }

        //public WorkingOrder(
        //    [NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
        //    [NotNullAttribute] string accountId
        //    )
        //{
        //    MapProperties(lsTradeSubscriptionData, accountId);

        //    _ = AccountId ?? throw new PrimaryKeyNullReferenceException(nameof(AccountId));
        //    _ = Epic ?? throw new EssentialPropertyNullReferenceException(nameof(Epic));
        //    _ = DealId ?? throw new EssentialPropertyNullReferenceException(nameof(DealId));
        //    _ = Direction ?? throw new EssentialPropertyNullReferenceException(nameof(Direction));
        //    _ = Currency ?? throw new EssentialPropertyNullReferenceException(nameof(Currency));
        //}
    }
}
