using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace time_track_service.Services
{
    public class SyncBackNotificationService : ISyncBackNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ContextLogger<ISyncBackNotificationService> _logger;
        private readonly ServicesSettings _servicesSettings;

        public SyncBackNotificationService(
            HttpClient httpClient, 
            IRequestContextAccessor requestContextAccessor, 
            ContextLogger<ISyncBackNotificationService> logger,
            ServicesSettings servicesSettings
            )
        {
            _httpClient = httpClient;
            _requestContextAccessor = requestContextAccessor;
            _servicesSettings = servicesSettings;
            _logger = logger;
        }

        public async Task<bool> SyncBackAsync<T>(SyncBackOperations operation, T newObject, T originalObject)
        {
            var entityName = newObject.GetType().Name;
            var changeSet = new ChangeSet<T>
            {
                Name = entityName,
                Operation = operation.ToString(),
                New = newObject,
                Original = originalObject
            };

            using var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(_servicesSettings.SyncBackUri + entityName),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(changeSet), Encoding.UTF8, "application/json"),
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _requestContextAccessor.RequestContext.AuthToken),
                }
            };

            httpRequest.Headers.Add(RequestHeaderName.TenantId, _requestContextAccessor.RequestContext.TenantId.ToString());
            httpRequest.Headers.Add(RequestHeaderName.SegmentId, _requestContextAccessor.RequestContext.SegmentId.ToString());

            _logger.LogTrace($"RequestContext Info - {JsonConvert.SerializeObject(_requestContextAccessor.RequestContext)}");
            _logger.LogTrace($"SyncBackAsync Request Info - URL: {httpRequest.RequestUri} Headers: {JsonConvert.SerializeObject(httpRequest.Headers)}");
            var result = await _httpClient.SendAsync(httpRequest);

            _logger.LogInformation($"SyncBackAsync result status:{result.StatusCode}");

            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError($"Error in syncBack process. {result.StatusCode} {result.ReasonPhrase} {result.Content.ReadAsStringAsync().Result}");
                return false;
            }

            return true;
        }
    }
}