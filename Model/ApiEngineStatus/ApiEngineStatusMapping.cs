namespace IGApi.Model
{
    public partial class ApiEngineStatus
    {
        public void MapProperties(DateTime timestamp)
        {
            IsAlive = timestamp;
        }
    }
}