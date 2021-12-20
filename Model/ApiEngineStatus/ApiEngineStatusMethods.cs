using IGApi.Common;

namespace IGApi.Model
{
    public partial class ApiEngineStatus
    {
        public static void SetIsAlive()
        {
            IGApiDbContext iGApiDbContext = new();
            _ = iGApiDbContext.ApiEngineStatus ?? throw new DBContextNullReferenceException(nameof(iGApiDbContext.ApiEngineStatus));

            iGApiDbContext.SaveApiEngineStatus();

            Task.Run(async ()=> await iGApiDbContext.SaveChangesAsync()).Wait();
        }
    }
}