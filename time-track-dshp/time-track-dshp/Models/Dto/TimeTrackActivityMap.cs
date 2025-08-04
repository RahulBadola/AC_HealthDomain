using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace time_track_dshp.Models.Dto
{
	public class TimeTrackActivityMap : MedCompassBase
	{
		[JsonProperty("Id")]
		[JsonPropertyName("Id")]
		[Required]
		public Guid TimeTrackActivityMapId { get; set; }

		public string TimeTrackActivityTypeKey { get; set; }

		public string TimeTrackSubActivityTypeKey { get; set; }
	}
}
