namespace IGApi.Model
{
    public partial class ApiEngineStatus
    {
        public void MapProperties()
        {
            IsAlive = DateTime.UtcNow;
        }
    }
}