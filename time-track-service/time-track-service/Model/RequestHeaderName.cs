namespace time_track_service.Model
{
    public static class RequestHeaderName
    {
        public const string Claims = "mcx-user-claims";
        public const string CorrelationId = "CorrelationId";
        public const string Domain = "Domain";
        public const string UserContextId = "mcx-user-context-id";
        public const string SegmentId = "SegmentId";
        public const string TenantId = "TenantId";
        public const string UserContext = "mcx-user-context";
        public const string UserId = "mcx-user-id";
        public const string SecurityModifications = "mcx-security-modifications";
        public const string Authorization = "Authorization";
        public const string FullName = "mcx-user-fullname";
        public const string HydrationSyncKey = "hydration-sync-key";
    }
}