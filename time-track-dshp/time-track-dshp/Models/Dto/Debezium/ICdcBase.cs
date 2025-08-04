using Newtonsoft.Json;
using System;
using time_track_dshp.Utils;

namespace time_track_dshp.Models.Dto.Debezium
{
    public interface ICdcBase
    {
#pragma warning disable IDE1006 // Naming Styles

        string __deleted { get; set; }

        string __op { get; set; }
        
        string __source_table { get; set; }
        
        [JsonConverter(typeof(UnixTimestampConverter))]
        DateTime? __source_ts_ms { get; set; }

#pragma warning restore IDE1006 // Naming Styles

        int ActiveFlag { get; set; }
    }
}
