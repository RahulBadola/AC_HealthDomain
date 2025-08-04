using System;

namespace time_track_dshp.Models.Configuration
{
    public class DomainConfiguration
    {
        public string Host { get; set; }
        public string Protocol { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string TenantId { get; set; }
        public string HydrationSyncKey { get; set; }

        public void ExpandEnvironmentVariables()
        {
            if (!string.IsNullOrEmpty(Host)) Host = Environment.ExpandEnvironmentVariables(Host);
            if (!string.IsNullOrEmpty(Protocol)) Protocol = Environment.ExpandEnvironmentVariables(Protocol);
            if (!string.IsNullOrEmpty(Path)) Path = Environment.ExpandEnvironmentVariables(Path);
            if (!string.IsNullOrEmpty(TenantId)) TenantId = Environment.ExpandEnvironmentVariables(TenantId);
            if (!string.IsNullOrEmpty(HydrationSyncKey)) HydrationSyncKey = Environment.ExpandEnvironmentVariables(HydrationSyncKey);
        }
    }
}
