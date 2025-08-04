using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Models.Dto;
using time_track_dshp.Models.Dto.Debezium;
namespace time_track_dshp.Services
{
    public class DomainMicroService : IDomainMicroService
    {
        private readonly ILogger<DomainMicroService> _logger;
        private readonly HttpClient _httpClient;
        private readonly DomainConfiguration _domainConfiguration;
        private readonly IAuthService _authService;

        public DomainMicroService(ILogger<DomainMicroService> logger, HttpClient httpClient, DomainConfiguration domainConfiguration, IAuthService authService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _domainConfiguration = domainConfiguration;
            _authService = authService;
        }

        public async Task<bool> SendRequest<T>(T obj, string endpoint) where T : IBaseEntity
        {
            var jwt = _authService.GetJwtAsync().Result;
            var content = JsonConvert.SerializeObject(obj);
            _logger.LogDebug($"Content JSON: {content}");
            var requestUri = new UriBuilder(_domainConfiguration.Protocol, _domainConfiguration.Host, _domainConfiguration.Port, endpoint);

            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri.Uri))
            {
                _logger.LogInformation($"Sending request: {request.RequestUri}");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                request.Headers.Add("TenantId", _domainConfiguration.TenantId.ToString());
                request.Headers.Add("CorrelationId", Guid.NewGuid().ToString());
                request.Headers.Add("hydration-sync-key", _domainConfiguration.HydrationSyncKey);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);

                _logger.LogInformation($"DomainMicroServiceResponse: {response}");
                return response.IsSuccessStatusCode;
            }
        }
        public async Task<bool> SendRequestBulk<T>(IEnumerable<T> obj, string domainName) where T : McBase
        {
            var jwt = _authService.GetJwtAsync().Result;
            var content = JsonConvert.SerializeObject(obj);
            _logger.LogDebug($"Content JSON: {content}");
            var requestUri = new UriBuilder(domainName);

            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri.Uri))
            {
                _logger.LogInformation($"Sending request: {request.RequestUri}");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                request.Headers.Add("TenantId", _domainConfiguration.TenantId.ToString());
                request.Headers.Add("CorrelationId", Guid.NewGuid().ToString());
                request.Headers.Add("hydration-sync-key", _domainConfiguration.HydrationSyncKey);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);

                _logger.LogInformation($"DomainMicroServiceResponse: {response}");
                return response.IsSuccessStatusCode;
            }
        }
        public string GetEndpoint(string domainName)
        {
            if (_domainConfiguration.Port.ToString() != "")
                return $"{_domainConfiguration.Protocol}://{_domainConfiguration.Host}:{_domainConfiguration.Port}/api/sync/{domainName}";
            else
                return $"{_domainConfiguration.Protocol}://{_domainConfiguration.Host}/api/sync/{domainName}";
        }


    }
}
