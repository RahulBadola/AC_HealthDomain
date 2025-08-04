using time_track_dshp.Models.Configuration;
using time_track_dshp.Utils.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public class RuleExecutionService : IRuleExecutionService
    {
        private readonly ILogger<RuleExecutionService> _logger;
        private readonly HttpClient _httpClient;
        private readonly RuleExecutionConfiguration _ruleExecutionConfiguration;
        private readonly IAuthService _authService;

        public RuleExecutionService(ILogger<RuleExecutionService> logger, HttpClient httpClient, RuleExecutionConfiguration ruleExecutionConfiguration, IAuthService authService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _ruleExecutionConfiguration = ruleExecutionConfiguration;
            _ruleExecutionConfiguration.ExpandEnvironmentVariables();
            _authService = authService;
        }

        public async Task<bool> SendRequest(object obj)
        {
            var jwt = _authService.GetJwtAsync().Result;
            var content = JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { ContractResolver = new RuleExecutionContractResolver() });
            _logger.LogDebug($"Content JSON: {content}");
            var requestUri = new UriBuilder(_ruleExecutionConfiguration.Protocol, _ruleExecutionConfiguration.Host, _ruleExecutionConfiguration.Port, _ruleExecutionConfiguration.Endpoint);

            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri.Uri))
            {
                _logger.LogInformation($"Sending request: {request.RequestUri}");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                request.Headers.Add("TenantId", _ruleExecutionConfiguration.TenantId.ToString());
                request.Headers.Add("CorrelationId", Guid.NewGuid().ToString());
                request.Headers.Add("api_key", _ruleExecutionConfiguration.ApiKey);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                var responseMessage = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"DomainMicroServiceResponse: {response.StatusCode} - {responseMessage} ({response})");
                return response.IsSuccessStatusCode;
            }
        }
    }
}
