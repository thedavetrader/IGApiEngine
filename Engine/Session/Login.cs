using System.Diagnostics;
using dto.endpoint.accountbalance;
using dto.endpoint.auth.session.v2;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Microsoft.EntityFrameworkCore;
using IGApi.RequestQueue;

namespace IGApi
{
    using static Log;
    public sealed partial class ApiEngine
    {
        private static bool isSessionLoginFunctionsAvailable = true;

        private AuthenticationResponse? _loginSessionInformation;

        public AuthenticationResponse LoginSessionInformation { get { return _loginSessionInformation ?? throw new NullReferenceException(nameof(LoginSessionInformation)); } }

        public bool IsLoggedIn { get { return _loginSessionInformation is not null; } }

        private readonly string? _env;
        private readonly string? _userName;
        private readonly string? _password;
        private readonly string? _apiKey;

        private void Login()
        {
            //  Prevent that other tasks also invokes (re-)login at same time.
            //  The login procedure most surely will fail, or at least yield unpredictable results.
            if (isSessionLoginFunctionsAvailable)
            {
                try
                {
                    isSessionLoginFunctionsAvailable = false;

                    var ar = new AuthenticationRequest { identifier = _userName, password = _password };
                    var response = IGRestApiClient.SecureAuthenticate(ar, _apiKey).UseManagedCall();

                    if (response is not null && response.Response is not null)
                    {
                        _loginSessionInformation = response.Response;

                        WriteLog(Columns("Logged in, current account: " + LoginSessionInformation.currentAccountId));

                        InitAccount(response);

                        LightstreamerLogin(response);
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

            void LightstreamerLogin(IgResponse<AuthenticationResponse> response)
            {
                bool connectionEstablished = false;
                const int retry = 10;

                for (int i = 1; i <= retry && !connectionEstablished; i++)
                {
                    try
                    {
                        WriteLog(Columns("establishing datastream connection"));
                        ConversationContext context = IGRestApiClient.GetConversationContext();

                        _ = context ?? throw new NullReferenceException(nameof(context));
                        _ = LoginSessionInformation.lightstreamerEndpoint ?? throw new NullReferenceException(nameof(LoginSessionInformation.lightstreamerEndpoint));

                        if ((context.apiKey is not null) && (context.xSecurityToken is not null) && (context.cst is not null))
                        {
                            connectionEstablished =
                        _iGStreamApiClient.Connect(
                            LoginSessionInformation.currentAccountId,
                            context.cst,
                            context.xSecurityToken,
                            context.apiKey,
                            LoginSessionInformation.lightstreamerEndpoint);

                            if (connectionEstablished)
                            {
                                WriteOk(Columns(String.Format("Connected to Lightstreamer. Endpoint ={0}",
                                                                    LoginSessionInformation.lightstreamerEndpoint)));

                                // Subscribe to Account Details and Trade Subscriptions...
                                Task.Run(() => ReSubscribeToAllEpicTick(), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();
                                Task.Run(() => SubscribeToTradeDetails(), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();
                                Task.Run(() => SubscribeToAccountDetails(), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait();
                            }
                            else
                            {
                                WriteError(Columns(String.Format(
                                    "Could NOT connect to Lightstreamer. Endpoint ={0}. Attempt = {1} ",
                                    LoginSessionInformation.lightstreamerEndpoint
                                    , i)));
                            }
                        }
                        else
                        {
                            throw new HttpRequestException("Failed to login to Lightstreamer endpoint.", null, response.StatusCode);
                        }
                    }
                    catch (Lightstreamer.DotNet.Client.SubscrException ex)
                    {
                        WriteException(ex);
                    }
                    catch (Exception ex)
                    {
                        WriteException(ex);
                        throw;
                    }
                }

                if (!connectionEstablished)
                    WriteLog(Columns($"Critical error. Could not connect to Lightstreamer after {retry} retries. Endpoint {LoginSessionInformation.lightstreamerEndpoint}"));
            }
        }

        void InitAccount(IgResponse<AuthenticationResponse> response)
        {
            string currentAccountId = "";
            var accountData = response.Response;
            currentAccountId = accountData.currentAccountId;

            if (accountData.accounts.Count > 0)
            {
                using ApiDbContext apiDbContext = new();

                apiDbContext.Accounts.Where(a => a.AccountId != currentAccountId && a.IsCurrent).ToList().ForEach(f => f.IsCurrent = false);

                foreach (var sessionAccount in accountData.accounts)
                {
                    if (sessionAccount.accountId == currentAccountId)
                    {
                        apiDbContext.SaveAccount(sessionAccount, accountData.accountInfo, isCurrent: true);
                    }
                    else
                        apiDbContext.SaveAccount(sessionAccount);
                }

                Task.Run(async () => await apiDbContext.SaveChangesAsync(_cancellationToken), _cancellationToken).ContinueWith(task => TaskException.CatchTaskIsCanceledException(task)).Wait(); // Use wait, avoiding object disposed DbContext operations are still running.
            }
        }
    }
}