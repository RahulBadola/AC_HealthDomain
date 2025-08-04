using System;

namespace type_lookup_service.Model
{
    public class RequestContext
    {
        public string CorrelationId { get; set; }

        public Guid? SegmentId { get; set; }

        public Guid? TenantId { get; set; }

        public string UserContext { get; set; }

        public Guid UserContextId { get; set; }

        public Guid UserId { get; set; }
    }
}