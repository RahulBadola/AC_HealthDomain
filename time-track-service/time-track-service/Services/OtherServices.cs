using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.ServiceDataObject;
using time_track_service.Utils;

namespace time_track_service.Services
{
    public class OtherServices : IOtherServices
    {
        private readonly HttpClient _httpClient;
        private readonly ContextLogger<IOtherServices> _logger;
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ServicesSettings _servicesSettings;

        public OtherServices(HttpClient httpClient, IRequestContextAccessor requestContextAccessor, ContextLogger<IOtherServices> contextLogger, ServicesSettings servicesSettings)
        {
            _httpClient = httpClient;
            _logger = contextLogger;
            _requestContextAccessor = requestContextAccessor;
            _servicesSettings = servicesSettings;
        }
        public async Task<Staff> ReadStaffAsync(Guid id)
        {
            Staff response = new Staff();
            try
            {
                using var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(_servicesSettings.UserServiceUri + "Staff/" + id),
                    Method = HttpMethod.Get,

                    Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _requestContextAccessor.RequestContext.AuthToken),
                }
                };

                httpRequest.Headers.Add(RequestHeaderName.TenantId, _requestContextAccessor.RequestContext.TenantId.ToString());
                httpRequest.Headers.Add(RequestHeaderName.SegmentId, _requestContextAccessor.RequestContext.SegmentId.ToString());

                var result = await _httpClient.SendAsync(httpRequest);
                _logger.LogInformation($"TaskService result:{result.StatusCode}");
                if (result.IsSuccessStatusCode)
                {
                    var stringData = await result.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<Staff>(stringData);
                }
                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError($"TaskService result:{ex.Message}");
                response = new Staff();
                return response;
            }
        }

        public async Task<List<Member>> GetMemberDetailsAsync(IList<Guid> memberIds)
        {
            var tasks = memberIds.Select(memberId => GetMemberDetailAsync(memberId));
            var membersInfo = await Task.WhenAll(tasks);
            return membersInfo.ToList();
        }

        public async Task<Member> GetMemberDetailAsync(Guid memberId)
        {
            Member response = new Member();
            try
            {
                using var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(_servicesSettings.MemberServiceUri + "Members/" + memberId),
                    Method = HttpMethod.Get,
                    Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _requestContextAccessor.RequestContext.AuthToken),
                }
                };

                httpRequest.Headers.Add(RequestHeaderName.TenantId, _requestContextAccessor.RequestContext.TenantId.ToString());
                httpRequest.Headers.Add(RequestHeaderName.SegmentId, _requestContextAccessor.RequestContext.SegmentId.ToString());

                var result = await _httpClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"MemberService result:{result.StatusCode}");
                    response = JsonConvert.DeserializeObject<Member>(content);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"MemberService result:{ex.Message}");
                response = new Member();
                return response;
            }

            return response;
        }

        public async Task<string> RequestTypeLookupDataAsync(List<string> typeNames, string url, string logPrefix)
        {
            try
            {
                string stringData = string.Empty;
                using var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                    Authorization = new AuthenticationHeaderValue("Bearer", _requestContextAccessor.RequestContext.AuthToken),
                    }

                };

                httpRequest.Headers.Add(RequestHeaderName.TenantId, _requestContextAccessor.RequestContext.TenantId.ToString());
                httpRequest.Headers.Add(RequestHeaderName.SegmentId, _requestContextAccessor.RequestContext.SegmentId.ToString());
                httpRequest.Headers.Add("typeNames", string.Join(",", typeNames));

                var result = await _httpClient.SendAsync(httpRequest);
                _logger.LogInformation($"{logPrefix} result:{result.StatusCode}");
                if (result.IsSuccessStatusCode)
                {
                     stringData = await result.Content.ReadAsStringAsync();
                }
                return stringData;

            }
            catch (Exception ex)
            {
                _logger.LogError($"{logPrefix} result:{ex.Message}");
                return null;
            }
        }

        public async Task<GenericLookup> GetLookUpDataAsync(List<string> typeNames)
        {
            _logger.LogDebug("OtherServices - GetLookUpDataAsync Begin");

            var stringData = await RequestTypeLookupDataAsync(typeNames, _servicesSettings.TypeLookUpGenericUri, "GenericLookUpService");
            return stringData == null ? new GenericLookup() : JsonConvert.DeserializeObject<GenericLookup>(stringData);
        }
        public async Task<MemberProgram> GetMemberProgramAsync(Guid id)
        {
            MemberProgram response = new MemberProgram();
            try
            {
                using var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(_servicesSettings.MemberProgramUri + "/memberprogram" + id),
                    Method = HttpMethod.Get,
                    Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _requestContextAccessor.RequestContext.AuthToken),
                }
                };

                httpRequest.Headers.Add(RequestHeaderName.TenantId, _requestContextAccessor.RequestContext.TenantId.ToString());
                httpRequest.Headers.Add(RequestHeaderName.SegmentId, _requestContextAccessor.RequestContext.SegmentId.ToString());

                var result = await _httpClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    var stringData = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"MemberProgramService result:{result.StatusCode}");
                    response = JsonConvert.DeserializeObject<MemberProgram>(stringData);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"MemberProgramService result:{ex.Message}");
                return new MemberProgram();
            }
        }
        public async Task<List<MemberProgram>> GetMemberProgramsAsync(Guid memberId)
        {
            List<MemberProgram> response = new List<MemberProgram>();
            try
            {
                using var httpRequest = new HttpRequestMessage
                {
                    RequestUri = new Uri(_servicesSettings.MemberProgramUri + memberId + "/programs"),
                    Method = HttpMethod.Get,
                    Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _requestContextAccessor.RequestContext.AuthToken),
                }
                };

                httpRequest.Headers.Add(RequestHeaderName.TenantId, _requestContextAccessor.RequestContext.TenantId.ToString());
                httpRequest.Headers.Add(RequestHeaderName.SegmentId, _requestContextAccessor.RequestContext.SegmentId.ToString());

                var result = await _httpClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    var stringData = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"MemberProgramService result:{result.StatusCode}");
                    response = JsonConvert.DeserializeObject<List<MemberProgram>>(stringData);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"MemberProgramService result:{ex.Message}");
                return new List<MemberProgram>();
            }
        }
    }
}
