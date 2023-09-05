using IGApi.Common;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private void Logout()
        {
            if (isSessionLoginFunctionsAvailable)
            {
                try
                {
                    isSessionLoginFunctionsAvailable = false;

                    if (IGRestApiClient is not null)
                    {

                        UnsubscribeFromAccountDetails();
                        UnsubscribeFromTradeDetails();
                        UnsubscribeFromAllEpicTick();
                        IGRestApiClient.logout();
                        _loginSessionInformation = null;
                        WriteLog(Columns("Logged out"));
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    throw;
                }
                finally
                {
                    isSessionLoginFunctionsAvailable = true;
                }
            }
        }
    }
}
