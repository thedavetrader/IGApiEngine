using IGApi.Common;
using Microsoft.EntityFrameworkCore;

namespace IGApi.Model
{
    public partial class ApiEngineStatus
    {
        public static void SetIsAlive(DateTime timestamp)
        {
            ApiDbContext apiDbContext = new();

            apiDbContext.SaveApiEngineStatus(timestamp);

            Task.Run(async ()=> await apiDbContext.SaveChangesAsync()).Wait();
        }
    }
}