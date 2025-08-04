using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using time_track_service.Model;

namespace time_track_service.Utils
{
    /// <summary>
    /// Provides access to current <see cref="RequestContext"/>
    /// </summary>
    /// <remarks>
    /// <see cref="RequestContext"/> will be created and stored in <see cref="HttpContext.Items"/>
    /// </remarks>
    public class RequestContextAccessor : IRequestContextAccessor
    {
        private const string RequestContextItemKey = "mc-request-context";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpContext HttpContext => _httpContextAccessor?.HttpContext;

        /// <summary>
        /// Initializes new <see cref="RequestContextAccessor"/> instance
        /// </summary>
        public RequestContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Current <see cref="RequestContext"/> from <see cref="HttpContext"/>
        /// </summary>
        public RequestContext RequestContext
        {
            get
            {
                var returnValue = GetRequestContext() ?? AddRequestContext();
                return returnValue;
            }
        }

        private RequestContext AddRequestContext()
        {
            var requestContext = CreateRequestContext();
            if (requestContext == null) return null;
            HttpContext.Items.Add(RequestContextItemKey, requestContext);
            return requestContext;
        }

        private RequestContext CreateRequestContext()
        {
            var httpRequest = HttpContext?.Request;
            if (httpRequest == null) return null;

            var returnValue = new RequestContext
            {
                Claims = ExtractHeaderValue(httpRequest, RequestHeaderName.Claims, null),
                CorrelationId = ExtractHeaderValue(httpRequest, RequestHeaderName.CorrelationId, HttpContext.TraceIdentifier),
                Domain = ExtractHeaderValue(httpRequest, RequestHeaderName.Domain, null),
                SegmentId = ExtractGuidValue(httpRequest, RequestHeaderName.SegmentId),
                TenantId = ExtractGuidValue(httpRequest, RequestHeaderName.TenantId),
                UserId = ExtractGuidValue(httpRequest, RequestHeaderName.UserId),
                UserContext = ExtractHeaderValue(httpRequest, RequestHeaderName.UserContext, null),
                UserContextId = ExtractGuidValue(httpRequest, RequestHeaderName.UserContextId),
                FullName = ExtractHeaderValue(httpRequest, RequestHeaderName.FullName, null),
                HydrationSyncKey = ExtractHeaderValue(httpRequest, RequestHeaderName.HydrationSyncKey, null),
                AuthToken = ExtractAuthToken(httpRequest, null)
            };

            return returnValue;
        }

        private Guid ExtractGuidValue(HttpRequest httpRequest, string keyName)
        {
            var result = httpRequest?.Headers == null
            ? null
            : httpRequest.Headers.TryGetValue(keyName, out var tmpValues)
            ? tmpValues.ToString().Split(",").FirstOrDefault()
            : null;

            return result != null ? Guid.TryParse(result, out var tmpValue) ? tmpValue : Guid.Empty : Guid.Empty;

        }

        private string ExtractHeaderValue(HttpRequest httpRequest, string keyName, string defaultValue)
        {
            if (httpRequest?.Headers == null)
            {
                return defaultValue;
            }            
            else
            {
                return httpRequest.Headers.TryGetValue(keyName, out var tmpValues) ? tmpValues.ToString().Split(",").FirstOrDefault() : defaultValue;
            }
        }

        private string ExtractAuthToken(HttpRequest httpRequest, string defaultValue)
        {
            var authorization = ExtractHeaderValue(httpRequest, RequestHeaderName.Authorization, null);
            return authorization != null ? authorization.Split(" ").LastOrDefault() : defaultValue;
        }

        private RequestContext GetRequestContext()
        {
            return HttpContext?.Items.ContainsKey(RequestContextItemKey) ?? false
            ? (RequestContext)HttpContext.Items[RequestContextItemKey]
            : null;
        }
    }
}