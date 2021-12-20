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
                        _loginSessionInformation = null;

                        UnsubscribeFromAccountDetails();
                        UnsubscribeFromTradeDetails();
                        UnsubscribeFromAllEpicTick();
                        IGRestApiClient.logout();
                        WriteLog(Messages("Logged out"));
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
