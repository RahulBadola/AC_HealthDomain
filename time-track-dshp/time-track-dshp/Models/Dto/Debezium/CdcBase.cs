using Newtonsoft.Json;
using System;
using time_track_dshp.Utils;

namespace time_track_dshp.Models.Dto.Debezium
{
    public class CdcBase : ICdcBase
    {
        public string __deleted { get; set; }

        public string __op { get; set; }

        public string __source_table { get; set; }

        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? __source_ts_ms { get; set; }

        public int ActiveFlag { get; set; }
    }
}
