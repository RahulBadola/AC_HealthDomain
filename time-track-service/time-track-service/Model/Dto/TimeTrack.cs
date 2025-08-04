using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using AssureCare.MedCompass.DataAuthorization.Models;

namespace time_track_service.Model.Dto
{
    [BsonIgnoreExtraElements]
    [SecureObject]
    public class TimeTrack : MedCompassBase
    {
        public bool CaseRecording { get; set; }

        public string Comment { get; set; }

        public DateTime EndDate { get; set; }

        public bool GenerateUnit { get; set; }

        public bool IsSystemTime { get; set; }

        public Guid? MemberId { get; set; }

        public Guid? MemberProgramId { get; set; }

        public string OtherDescription { get; set; }

        public string ProgramTypeKey { get; set; }

        public string Reason { get; set; }

        public Guid? RowFilterId { get; set; }

        public Guid? SecurityUserId { get; set; }

        public Guid? ServiceAuthId { get; set; }

        public string ServicePlanFundingSourceTypeKey { get; set; }

        public string ServiceTypeKey { get; set; }

        public int? ServiceUnits { get; set; }

        public DateTime StartDate { get; set; }

        public Guid? SyncLockedBy { get; set; }

        public Guid? SyncLockedById { get; set; }

        public string SyncLockedByName { get; set; }

        public DateTime? SyncLockedOn { get; set; }

        public string SyncLockStateTypeKey { get; set; }

        public string TimeTrackActivityTypeKey { get; set; }

        public string TimeTrackSubActivityTypeKey { get; set; }

        public int? TotalTime { get; set; }

        public int? TravelDuration { get; set; }

        public int? TravelMiles { get; set; }

        public BsonDocument Udf { get; set; }

        public bool VolunteerDriver { get; set; }

    }
}