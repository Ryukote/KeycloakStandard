using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class RealmRole
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public int Description { get; set; }

        [JsonProperty("composite")]
        public bool IsComposite { get; set; }

        [JsonProperty("clientRole")]
        public bool IsClientRole { get; set; }

        [JsonProperty("containerId")]
        public string ContainerId { get; set; }
    }
}
