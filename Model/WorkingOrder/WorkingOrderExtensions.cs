using System.Diagnostics.CodeAnalysis;
using dto.endpoint.workingorders.get.v2;
using IGApi.Common;

namespace IGApi.Model
{
    internal static partial class DtoModelExtensions
    {
        public static WorkingOrder? SaveWorkingOrder(
            [NotNullAttribute] this ApiDbContext apiDbContext,
            [NotNullAttribute] WorkingOrderData WorkingOrderData,
            [NotNullAttribute] string accountId
            )
        {
            var currentWorkingOrder = Task.Run(async () => await apiDbContext.WorkingOrders.FindAsync(accountId, WorkingOrderData.dealId)).Result;

            if (currentWorkingOrder is not null)
                currentWorkingOrder.MapProperties(WorkingOrderData, accountId);
            else
                currentWorkingOrder = apiDbContext.WorkingOrders.Add(new WorkingOrder(WorkingOrderData, accountId)).Entity;

            return currentWorkingOrder;
        }
    }
}