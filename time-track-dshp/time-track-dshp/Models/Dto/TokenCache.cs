using System;

namespace time_track_dshp.Models.Dto
{
    public class TokenCache
    {
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public int TimeToLiveInSeconds { get; set; }
    }
}
