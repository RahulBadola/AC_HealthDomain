using System;
using AssureCare.MedCompass.DataAuthorization.Models;

namespace time_track_service.Model.Dto.Legacy
{
    [SecureObject("TimeTrack")]
    public class LegacyTimeTrackingModel : IRowFiltered
    {
        public string Activity { get; set; }
        public string AdditionalComments { get; set; }
        public string Case { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Date { get; set; }
        public string EndTime { get; set; }
        public string FundingSource { get; set; }
        [SecureField("TimeTrackId")]
        public Guid? Id { get; set; }
        public string InsertedByName { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string Member { get; set; }
        public Guid? MemberId { get; set; }
        public Guid? MemberProgramId { get; set; }
        public string ProgramType { get; set; }
        public string Reason { get; set; }
        public Guid SegmentId { get; set; }
        public string Service { get; set; }
        public int? ServiceUnits { get; set; }
        public Guid? SecurityUserId { get; set; }
        public Guid? StaffGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartTime { get; set; }
        public string SubActivity { get; set; }
        public int? TotalTime { get; set; }
        public int? TravelDuration { get; set; }
        public int? TravelMiles { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedByName { get; set; }
        public bool? VolunteerDriver { get; set; }
        public Guid? VoidedReasonId { get; set; }

        public Guid? RowFilterId => this.StaffGuid;
    }
}
