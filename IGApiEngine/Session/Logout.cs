using IGApiEngine.Common;

namespace IGApi
{
    public sealed partial class IGApiEngine
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
                    IGRestApiClient.logout();

                    Log.WriteLine("Logged out");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }
    }
}
