using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Models.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace time_track_dshp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthConfiguration _authConfiguration;
        private TokenCache _tokenCache;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AuthConfiguration authConfiguration, ILogger<AuthService> logger, HttpClient httpClient)
        {
            _authConfiguration = authConfiguration;
            _tokenCache = null;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<string> GetJwtAsync()
        {
            _logger.LogDebug("AuthService GetJwtAsync: Enter");
            if (_tokenCache != null && _tokenCache.Created.AddSeconds(_tokenCache.TimeToLiveInSeconds) > DateTime.UtcNow)
            {
                _logger.LogDebug("AuthService GetJwtAsync: using cached token");
                return _tokenCache.Token;
            }
            _logger.LogDebug("AuthService GetJwtAsync: building new token");
            var authToken = Base64Encode($"{_authConfiguration.ClientId}:{_authConfiguration.ClientSecret}");

            using var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_authConfiguration.OAuthEndPoint),
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Basic", authToken)
                },
                Content = new StringContent(
                    $"grant_type=password&scope=openid&username={_authConfiguration.ClientUsername}&password={_authConfiguration.ClientPassword}",
                    Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            _logger.LogDebug("AuthService GetJwtAsync: Calling Okta");
            var response = await _httpClient.SendAsync(httpRequest);
            _logger.LogDebug("AuthService GetJwtAsync: Getting Token");
            var tokenPackage = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("AuthService GetJwtAsync: Building Token");
            var tokenRequested = JsonConvert.DeserializeObject<TokenResponses>(tokenPackage);
            _tokenCache = new TokenCache
            {
                Token = tokenRequested.AccessToken,
                Created = DateTime.UtcNow,
                TimeToLiveInSeconds = tokenRequested.ExpiresIn
            };

            _logger.LogDebug($"AuthService GetJwtAsync: AccessToken - {tokenRequested.AccessToken}");

            return tokenRequested.AccessToken;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
