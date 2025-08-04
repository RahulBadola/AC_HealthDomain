using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using time_track_dshp.Attributes;

namespace time_track_dshp.Models.Dto
{
	public class TimeTrack : MedCompassBase
	{
		[JsonProperty("Id")]
		[JsonPropertyName("Id")]
		[Required]
		public Guid TimeTrackId { get; set; }

		public Guid? SecurityUserId { get; set; }

		public string TimeTrackActivityTypeKey { get; set; }

		public string ServiceTypeKey { get; set; }

		public string ProgramTypeKey { get; set; }

		public string ServicePlanFundingSourceTypeKey { get; set; }

		public string TimeTrackSubActivityTypeKey { get; set; }

		public Guid? ServiceAuthId { get; set; }

		public Guid? MemberId { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public int? TotalTime { get; set; }

		public bool GenerateUnit { get; set; }

		public bool CaseRecording { get; set; }

		public bool VolunteerDriver { get; set; }

		public bool IsSystemTime { get; set; }

		public int? ServiceUnits { get; set; }

		public int? TravelMiles { get; set; }

		public int? TravelDuration { get; set; }

		public string Reason { get; set; }

		public string OtherDescription { get; set; }

		public string Comment { get; set; }

		public Guid? MemberProgramId { get; set; }

		public string SyncLockStateTypeKey { get; set; }

		public Guid? SyncLockedBy { get; set; }

		public Guid? SyncLockedById { get; set; }

		public DateTime? SyncLockedOn { get; set; }

		public Guid? RowFilterId { get; set; }

		public Guid? ObjectId { get; set; }

		public string ObjectTypeKey { get; set; }

		public bool IsEndDateOnCallLog { get; set; }

		[NonSqlProperty]
		public string SyncLockedByName { get; set; }
	}
}
