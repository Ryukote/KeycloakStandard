using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class ClientRole
    {
        [JsonProperty("id")]
        public string Id { get; set;}
        [JsonProperty("name")]
        public string Name { get; set;}
    }
}
