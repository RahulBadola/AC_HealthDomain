using time_track_dshp.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using time_track_dshp.Utils;

namespace time_track_dshp.Models.Dto.Debezium
{
	public class TimeTrackActivityMap : McBase
	{
		
		public Guid Id =>TimeTrackActivityMapId;
		public Guid TimeTrackActivityMapId { get; set; }

		public string TimeTrackActivityTypeKey { get; set; }

		public string TimeTrackSubActivityTypeKey { get; set; }
	}
}
