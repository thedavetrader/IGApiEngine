using System.Diagnostics;
using dto.endpoint.accountbalance;
using dto.endpoint.auth.session.v2;
using IGApi.Model;
using IGApiEngine.Common;
using IGWebApiClient;

namespace IGApi
{
    public sealed partial class IGApiEngine
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

                _ = response ?? throw new RestCallNullException(nameof(IGRestApiClient.SecureAuthenticate));

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
                Log.WriteLine(ex.ToString());
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
                            SubscribeToAccountDetails();
                            //SubscribeToTradeSubscription();
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
                        Log.WriteLine(ex.Message);
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

                //  Account
                if (accountData.accounts.Count > 0)
                {
                    List<IGApiAccountDetails> iGApiAccountDetails = new();

                    foreach (var sessionAccount in accountData.accounts)
                    {
                        if (sessionAccount.preferred)
                            currentAccountId = sessionAccount.accountId;

                        iGApiAccountDetails.Add(new IGApiAccountDetails(sessionAccount));
                    }

                    Task.Run(async () => await iGApiAccountDetails.InsertOrUpdateAsync());
                }

                // Accountbalance data
                _ = accountData.accountInfo ?? throw new RestCallNullException(nameof(accountData.accountInfo));
                {
                    Task.Run(async () => await new IGApiAccountBalance(accountData.accountInfo, currentAccountId).InsertOrUpdateAsync());
                }
            }
        }
    }
}