using System.Collections.Generic;

namespace time_track_service.Model.ServiceDataObject
{
    public class GenericLookup
    {
        public List<Datum> TimeTrackActivityType { get; set; }
        public List<Datum> TimeTrackSubActivityType { get; set; }
        public List<Datum> ProgramStatusType { get; set; }
        public List<SiteConfiguration> siteConfiguration { get; set; }
        public List<Datum> ProgramType { get; set; }
        public List<Datum> ServicePlanFundingSourceType { get; set; }
        public List<Datum> ServiceType { get; set; }
        public GenericLookup()
        {
            TimeTrackActivityType = new List<Datum>();
            TimeTrackSubActivityType = new List<Datum>();
            ProgramStatusType = new List<Datum>();
            siteConfiguration = new List<SiteConfiguration>();
            ProgramType = new List<Datum>();
            ServicePlanFundingSourceType = new List<Datum>();
            ServiceType = new List<Datum>();
        }
    }
}
