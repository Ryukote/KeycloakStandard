using Newtonsoft.Json;

namespace KeycloakStandard.Models.User
{
    public class ResetPassword
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
