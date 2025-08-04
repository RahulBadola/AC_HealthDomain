using System;
using time_track_service.Utils.Attributes;

namespace time_track_service.Model
{
    public class RequestContext
    {
        [DynamicHeader(RequestHeaderName.Claims, true, false)]
        public string Claims { get; set; }

        [DynamicHeader(RequestHeaderName.CorrelationId, false, false)]
        public string CorrelationId { get; set; }

        [DynamicHeader(RequestHeaderName.Domain, false, false)]
        public string Domain { get; set; }

        [DynamicHeader(RequestHeaderName.SegmentId, true, false)]
        public Guid SegmentId { get; set; }

        [DynamicHeader(RequestHeaderName.TenantId, true, false)]
        public Guid TenantId { get; set; }

        [DynamicHeader(RequestHeaderName.UserContext, true, false)]
        public string UserContext { get; set; }

        [DynamicHeader(RequestHeaderName.UserContextId, true, false)]
        public Guid UserContextId { get; set; }

        [DynamicHeader(RequestHeaderName.UserId, true, false)]
        public Guid UserId { get; set; }

        [DynamicHeader(RequestHeaderName.Authorization, false, false)]
        public string AuthToken { get; set; }

        [DynamicHeader(RequestHeaderName.FullName, false, false)]
        public string FullName { get; set; }

        [DynamicHeader(RequestHeaderName.HydrationSyncKey, false, false)]
        public string HydrationSyncKey { get; set; }
    }
}