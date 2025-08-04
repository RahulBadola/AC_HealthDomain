using System;

namespace time_track_service.Model.ServiceDataObject
{
    public class SiteConfiguration
    {
        public string key { get; set; }
        public string configValue { get; set; }
        public int activeFlag { get; set; }
        public string configKey { get; set; }
    }

}
