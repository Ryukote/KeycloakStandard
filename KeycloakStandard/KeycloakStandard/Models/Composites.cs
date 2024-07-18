using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class Composites
    {
        [JsonProperty("realm")]
        public string Realm { get; set; }
    }
}
