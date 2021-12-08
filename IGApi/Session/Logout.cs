using IGApi.Common;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        public void Stop()
        {
            Logout();   
        }

        private void Logout()
        {
            try
            {
                if (IGRestApiClient is not null)
                {
                    UnsubscribeFromAccountDetails();
                    UnsubscribeFromTradeDetails();
                    UnsubscribeFromAllEpicTick();
                    IGRestApiClient.logout();

                    Log.WriteLine("Logged out");
                }
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(Logout));
                throw;
            }
        }
    }
}
