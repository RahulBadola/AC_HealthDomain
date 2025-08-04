using System;
using time_track_service.Model.Dto;

namespace time_track_service.Model.Legacy
{
    public class LegacyMemberTimeTrackingsItemModel : MedCompassBase
    {
        public string Activity { get; set; }
        public string Case { get; set; }
        public DateTime? Date { get; set; }
        public string FundingSource { get; set; }
        public string Member { get; set; }
        public string ProgramType { get; set; }
        public string Reason { get; set; }
        public string SubActivity { get; set; }
        public string Service { get; set; }
        public int? ServiceUnits { get; set; }
        public int? TotalTime { get; set; }
    }
}
