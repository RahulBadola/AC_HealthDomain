using System;

namespace time_track_dshp.Models.Configuration
{
    public class RuleExecutionConfiguration
    {
        public string Host { get; set; }
        public string Protocol { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string Endpoint { get; set; }
        public Guid TenantId { get; set; }
        public string ApiKey { get; set; }

        public void ExpandEnvironmentVariables()
        {
            if (!string.IsNullOrEmpty(Host)) Host = Environment.ExpandEnvironmentVariables(Host);
            if (!string.IsNullOrEmpty(Protocol)) Protocol = Environment.ExpandEnvironmentVariables(Protocol);
            if (!string.IsNullOrEmpty(Path)) Path = Environment.ExpandEnvironmentVariables(Path);
            if (!string.IsNullOrEmpty(Endpoint)) Endpoint = Environment.ExpandEnvironmentVariables(Endpoint);
        }
    }
}
