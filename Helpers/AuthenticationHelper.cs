using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace groveale
{
    public class AuthenticationHelper
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _refreshToken;
        private readonly string _scope;
        private readonly string _tokenEndpoint;

        public AuthenticationHelper(string clientId, string clientSecret, string refreshToken, string scope, string tenantId)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _refreshToken = refreshToken;
            _scope = scope;
            _tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
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
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    return tokenResponse.AccessToken;
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
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
        }
    }

}
