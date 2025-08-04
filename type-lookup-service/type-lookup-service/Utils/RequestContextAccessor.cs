using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using type_lookup_service.Model;

namespace type_lookup_service.Utils
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
                CorrelationId = ExtractHeaderValue(httpRequest, RequestHeaderName.CorrelationId, null),
                SegmentId = ExtractNullableGuidValue(httpRequest, RequestHeaderName.SegmentId),
                TenantId = ExtractNullableGuidValue(httpRequest, RequestHeaderName.TenantId),
                UserId = ExtractGuidValue(httpRequest, RequestHeaderName.UserId),
                UserContext = ExtractHeaderValue(httpRequest, RequestHeaderName.UserContext, null),
                UserContextId = ExtractGuidValue(httpRequest, RequestHeaderName.UserContextId)
            };

            return returnValue;
        }

        private Guid ExtractGuidValue(HttpRequest httpRequest, string keyName)
        {
            var value = ExtractHeaderValue(httpRequest, keyName, null);
            return Guid.TryParse(value, out var g) ? g : Guid.Empty;
        }

        private string ExtractHeaderValue(HttpRequest httpRequest, string keyName, string defaultValue)
        {
            if (httpRequest?.Headers == null) return defaultValue;
            if (httpRequest.Headers.TryGetValue(keyName, out var h)) return h.FirstOrDefault();
            return defaultValue;
        }

        private Guid? ExtractNullableGuidValue(HttpRequest httpRequest, string keyName)
        {
            var value = ExtractHeaderValue(httpRequest, keyName, null);
            return Guid.TryParse(value, out var g) ? g : (Guid?)null;
        }

        private RequestContext GetRequestContext()
                => HttpContext?.Items.ContainsKey(RequestContextItemKey) ?? false
                ? (RequestContext)HttpContext.Items[RequestContextItemKey]
                : null;
    }
}