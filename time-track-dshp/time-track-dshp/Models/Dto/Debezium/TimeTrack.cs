using time_track_dshp.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using time_track_dshp.Utils;

namespace time_track_dshp.Models.Dto.Debezium
{
	public class TimeTrack : McBase
	{
		
		public Guid Id =>TimeTrackId;
		public Guid TimeTrackId { get; set; }

		public Guid? SecurityUserId { get; set; }

		public string TimeTrackActivityTypeKey { get; set; }

		public string ServiceTypeKey { get; set; }

		public string ProgramTypeKey { get; set; }

		public string ServicePlanFundingSourceTypeKey { get; set; }

		public string TimeTrackSubActivityTypeKey { get; set; }

		public Guid? ServiceAuthId { get; set; }

		public Guid? MemberId { get; set; }

		[JsonConverter(typeof(UnixTimestampConverter))]
		public DateTime StartDate { get; set; }

		[JsonConverter(typeof(UnixTimestampConverter))]
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

		[JsonConverter(typeof(UnixTimestampConverter))]
		public DateTime? SyncLockedOn { get; set; }

		public Guid? RowFilterId { get; set; }

		public Guid? ObjectId { get; set; }

		public string ObjectTypeKey { get; set; }

		public bool IsEndDateOnCallLog { get; set; }

		
		public string SyncLockedByName { get; set; }
	}
}
