using System.Diagnostics;
using dto.endpoint.accountbalance;
using dto.endpoint.auth.session.v2;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Microsoft.EntityFrameworkCore;
using IGApi.RestRequest;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        private static bool isSessionFunctionsAvailable = true;

        public AuthenticationResponse? LoginSessionInformation { get; set; }

        private readonly string? _env;
        private readonly string? _userName;
        private readonly string? _password;
        private readonly string? _apiKey;

        public void Start()
        {
            Login();
        }

        public void Relogin()
        {
            Logout();

            Login();
        }

        private void Login()
        {
            //  Prevent that other tasks also invokes (re-)login at same time.
            //  The login procedure most surely will fail, or at least yield unpredictable results.
            if (isSessionFunctionsAvailable)
            {
                try
                {
                    isSessionFunctionsAvailable = false;

                    var ar = new AuthenticationRequest { identifier = _userName, password = _password };
                    var response = IGRestApiClient.SecureAuthenticate(ar, _apiKey).UseManagedCall(); 

                    if (response is not null)
                    {

                        LoginSessionInformation = response.Response;

                        Log.WriteLine("Logged in, current account: " + LoginSessionInformation.currentAccountId);

                        InitAccount(response);

                        LightstreamerLogin(response);
                    }
                    else
                        throw new RestCallNullReferenceException(nameof(IGRestApiClient.SecureAuthenticate));
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex, nameof(Login));
                    throw;
                }
                finally
                {
                    isSessionFunctionsAvailable = true;
                }
            }

            void LightstreamerLogin(IgResponse<AuthenticationResponse> response)
            {
                Log.WriteLine("establishing datastream connection");
                ConversationContext context = IGRestApiClient.GetConversationContext();

                _ = context ?? throw new NullReferenceException(nameof(context));
                _ = LoginSessionInformation.lightstreamerEndpoint ?? throw new NullReferenceException(nameof(LoginSessionInformation.lightstreamerEndpoint));

                if ((context.apiKey is not null) && (context.xSecurityToken is not null) && (context.cst is not null))
                {
                    bool connectionEstablished = false;
                    const int retry = 10;

                    for (int i = 1; i <= retry && !connectionEstablished; i++)
                    {
                        try
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
                                Log.WriteLine(String.Format("Connecting to Lightstreamer. Endpoint ={0}",
                                                                    LoginSessionInformation.lightstreamerEndpoint));

                                // Subscribe to Account Details and Trade Subscriptions...
                                Task.Run(() => SubscribeToAllEpicTick()).Wait();
                                Task.Run(() => SubscribeToTradeDetails()).Wait();
                                Task.Run(() => SubscribeToAccountDetails()).Wait();
                            }
                            else
                            {
                                Log.WriteLine(String.Format(
                                    "Could NOT connect to Lightstreamer. Endpoint ={0}. Attempt = {1} ",
                                    LoginSessionInformation.lightstreamerEndpoint
                                    , i));
                            }
                        }
                        catch (Lightstreamer.DotNet.Client.SubscrException ex)
                        {
                            Log.WriteException(ex, nameof(LightstreamerLogin));
                        }
                        catch (Exception ex)
                        {
                            Log.WriteException(ex, nameof(LightstreamerLogin));
                            throw;
                        }
                    }

                    if (!connectionEstablished)
                        Log.WriteLine($"Critical error. Could not connect to Lightstreamer after {retry} retries. Endpoint {LoginSessionInformation.lightstreamerEndpoint}");
                }
                else
                {
                    throw new HttpRequestException("Failed to login to Lightstreamer endpoint.", null, response.StatusCode);
                }
            }

            static void InitAccount(IgResponse<AuthenticationResponse> response)
            {
                string currentAccountId = "";
                var accountData = response.Response;

                if (accountData.accounts.Count > 0)
                {
                    using IGApiDbContext iGApiDbContext = new();

                    foreach (var sessionAccount in accountData.accounts)
                    {
                        if (sessionAccount.preferred)
                        {
                            currentAccountId = sessionAccount.accountId;
                            iGApiDbContext.SaveAccount(sessionAccount, accountData.accountInfo);
                        }
                        else
                            iGApiDbContext.SaveAccount(sessionAccount);
                    }

                    Task.Run(async () => await iGApiDbContext.SaveChangesAsync()).Wait(); // Use wait, avoiding object disposed DbContext operations are still running.
                }
            }
        }
    }
}