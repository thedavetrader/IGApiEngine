using IGApi.Common;

namespace IGApi.Model
{
    public partial class ApiEngineStatus
    {
        public static void SetIsAlive()
        {
            ApiDbContext apiDbContext = new();
            _ = apiDbContext.ApiEngineStatus ?? throw new DBContextNullReferenceException(nameof(apiDbContext.ApiEngineStatus));

            apiDbContext.SaveApiEngineStatus();

            Task.Run(async ()=> await apiDbContext.SaveChangesAsync()).Wait();
        }
    }
}