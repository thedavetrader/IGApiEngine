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

            var currentWorkingOrder = Task.Run(async () => await iGApiDbContext.WorkingOrders.FindAsync(accountId, WorkingOrderData.dealId)).Result;

            if (currentWorkingOrder is not null)
                currentWorkingOrder.MapProperties(WorkingOrderData, accountId);
            else
                currentWorkingOrder = iGApiDbContext.WorkingOrders.Add(new WorkingOrder(WorkingOrderData, accountId)).Entity;

            return currentWorkingOrder;
        }
    }
}