using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace groveale
{
    public class AuthenticationHelper
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _refreshToken;
        private readonly string _scope;
        private readonly string _tokenEndpoint;
        private readonly string _secretName;

        private KeyVaultHelper _keyVaultHelper;

        public AuthenticationHelper(string clientId, string clientSecret, string scope, string tenantId, KeyVaultHelper keyVaultHelper)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = scope;
            _tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            _secretName = "SPOProjectOnlineRefreshToken";
            _keyVaultHelper = keyVaultHelper;
            _refreshToken = _keyVaultHelper.GetSecret(_secretName);
        }

        public async Task<string> GetAccessToken()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "refresh_token", _refreshToken },
                    { "grant_type", "refresh_token" },
                    { "scope", _scope }
                };

                var response = await httpClient.PostAsync(_tokenEndpoint, new FormUrlEncodedContent(request));

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    
                    // Deserialize the JSON string into a TokenResponse object
                    TokenResponse tokenResponseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

                    if (_refreshToken != tokenResponseObject.refresh_token)
                    {
                        _keyVaultHelper.SetSecret(_secretName, tokenResponseObject.refresh_token);
                    }

                    return tokenResponseObject.access_token;
                }
                else
                {
                    // Handle error
                    throw new InvalidOperationException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        public class TokenResponse
        {
            public string token_type { get; set; }
            public string scope { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string id_token { get; set; }
        }
    }

}