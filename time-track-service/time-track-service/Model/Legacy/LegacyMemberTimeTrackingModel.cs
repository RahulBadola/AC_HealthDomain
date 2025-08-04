using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using time_track_service.Model.Dto;
using time_track_service.Model.ServiceDataObject;
using AssureCare.MedCompass.DataAuthorization.Models;

namespace time_track_service.Model.Legacy
{
    [SecureObject("TimeTrack")]
    public class LegacyMemberTimeTrackingModel : IRowFiltered
    {
        public LegacyMemberTimeTrackingModel()
        {
            CaseList = new List<MemberCaseModel>();
        }


        public string Activity { get; set; }

        public string AdditionalComments { get; set; }

        public bool? CareProgramRequired { get; set; }

        public string Case { get; set; }

        public List<MemberCaseModel> CaseList { get; set; }

        public DateTime EndDate { get; set; }

        [SecureField("TimeTrackId")]
        public Guid? Id { get; set; }

        public Guid? MemberProgramGuid { get; set; }

        public DateTime StartDate { get; set; }

        
        public DateTime? Date { get; set; }
        
        public string EndTime { get; set; }

        public string FundingSource { get; set; }

        public string Member { get; set; }
        
        public Guid? MemberGuid { get; set; }

        public DateTime? InsertedOn { get; set; }
        
        public string InsertedByName { get; set; }
        
        public string ProgramType { get; set; }
        
        public string Reason { get; set; }

        public Guid SegmentId { get; set; }

        public string Service { get; set; }
        
        public int? ServiceUnits { get; set; }

        public string StartTime { get; set; }

        public string SubActivity { get; set; }

        public int? TotalTime { get; set; }

        public int? TravelDuration { get; set; }

        public int? TravelMiles { get; set; }

        public JObject Udf { get; set; }

        public string UpdatedByName { get; set; }

        public DateTime? UpdatedOn { get; set; }
        

        public bool? VolunteerDriver { get; set; }

        public Guid? VoidedReasonId { get; set; }

        public Guid? RowFilterId => this.MemberGuid;
    }
}
