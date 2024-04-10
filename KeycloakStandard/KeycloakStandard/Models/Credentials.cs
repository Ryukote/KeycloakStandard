using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class Credentials
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
    }
}
