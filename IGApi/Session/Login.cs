using System.Diagnostics;
using dto.endpoint.accountbalance;
using dto.endpoint.auth.session.v2;
using IGApi.Model;
using IGApi.Common;
using IGWebApiClient;
using Microsoft.EntityFrameworkCore;

namespace IGApi
{
    public sealed partial class ApiEngine
    {
        public AuthenticationResponse? LoginSessionInformation { get; set; }

        private readonly string? _env;
        private readonly string? _userName;
        private readonly string? _password;
        private readonly string? _apiKey;

        public void Start()
        {
            Login();
        }

        private void Login()
        {
            var ar = new AuthenticationRequest { identifier = _userName, password = _password };

            try
            {
                var response = Task.Run(async () => await IGRestApiClient.SecureAuthenticate(ar, _apiKey)).Result;
                LoginSessionInformation = response.Response;

                _ = response ?? throw new RestCallNullReferenceException(nameof(IGRestApiClient.SecureAuthenticate));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Log.WriteLine("Logged in, current account: " + LoginSessionInformation.currentAccountId);

                    InitAccount(response);

                    LightstreamerLogin(response);
                }
                else
                    throw new HttpRequestException("Failed login authentication.", null, response.StatusCode);
            }
            catch (Exception ex)
            {
                Log.WriteException(ex, nameof(Login));
                throw;
            }

            void LightstreamerLogin(IgResponse<AuthenticationResponse> response)
            {
                Log.WriteLine("establishing datastream connection");
                ConversationContext context = IGRestApiClient.GetConversationContext();

                _ = context ?? throw new NullReferenceException(nameof(context));
                _ = LoginSessionInformation.lightstreamerEndpoint ?? throw new NullReferenceException(nameof(LoginSessionInformation.lightstreamerEndpoint));

                if ((context.apiKey is not null) && (context.xSecurityToken is not null) && (context.cst is not null))
                {
                    try
                    {
                        var connectionEstablished =
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
                            Task.Run(()=> SubscribeToAccountDetails());
                            Task.Run(()=> SubscribeToTradeDetails());
                        }
                        else
                        {
                            Log.WriteLine(String.Format(
                                "Could NOT connect to Lightstreamer. Endpoint ={0}",
                                LoginSessionInformation.lightstreamerEndpoint));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteException(ex, nameof(LightstreamerLogin));
                        throw;
                    }
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

                    Task.Run(async ()=> await iGApiDbContext.SaveChangesAsync()).Wait(); // Use wait, avoiding object disposed DbContext operations are still running.
                }
            }
        }
    }
}