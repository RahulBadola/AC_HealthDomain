using Newtonsoft.Json;

namespace time_track_dshp.Models.Dto
{
    public class TokenResponses
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("id_token")]
        public string TokenId { get; set; }
    }
}
