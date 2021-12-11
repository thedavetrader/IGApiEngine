using System.Diagnostics.CodeAnalysis;
using dto.endpoint.workingorders.get.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static WorkingOrder? SaveWorkingOrder(
            [NotNullAttribute] this IGApiDbContext iGApiDbContext,
            [NotNullAttribute] WorkingOrderData WorkingOrderData,
            [NotNullAttribute] string accountId
            )
        {
            _ = iGApiDbContext.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.WorkingOrders));

            var WorkingOrder = Task.Run(async () => await iGApiDbContext.WorkingOrders.FindAsync(accountId, WorkingOrderData.dealId)).Result;

            if (WorkingOrder is not null)
                WorkingOrder.MapProperties(WorkingOrderData, accountId);
            else
                WorkingOrder = iGApiDbContext.WorkingOrders.Add(new WorkingOrder(WorkingOrderData, accountId)).Entity;

            return WorkingOrder;
        }

        //public static WorkingOrder? SaveWorkingOrder(
        //[NotNullAttribute] this IGApiDbContext iGApiDbContext,
        //[NotNullAttribute] LsTradeSubscriptionData lsTradeSubscriptionData,
        //[NotNullAttribute] string accountId
        //)
        //{
        //    _ = iGApiDbContext.WorkingOrders ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.WorkingOrders));

        //    var WorkingOrder = Task.Run(async () => await iGApiDbContext.WorkingOrders.FindAsync(accountId, lsTradeSubscriptionData.dealId)).Result;

        //    if (WorkingOrder is not null)
        //        WorkingOrder.MapProperties(lsTradeSubscriptionData, accountId);
        //    else
        //        WorkingOrder = iGApiDbContext.WorkingOrders.Add(new WorkingOrder(lsTradeSubscriptionData, accountId)).Entity;

        //    return WorkingOrder;
        //}
    }
}