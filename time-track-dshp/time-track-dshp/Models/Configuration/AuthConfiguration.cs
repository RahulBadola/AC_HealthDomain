using System;

namespace time_track_dshp.Models.Configuration
{
    public class AuthConfiguration
    {
        private string _oAuthEndpoint;

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientUsername { get; set; }
        public string ClientPassword { get; set; }

        public string OAuthEndPoint
        {
            get => _oAuthEndpoint;
            set => _oAuthEndpoint = Environment.ExpandEnvironmentVariables(value);
        }
    }
}
