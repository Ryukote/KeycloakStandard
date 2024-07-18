using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class RoleRepresentation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("scopeParamRequired")]
        public bool ScopeParamRequired { get; set; }
        [JsonProperty("composite")]
        public bool Composite { get; set; }
        //[JsonProperty("composites")]
        //public Composites Composites { get; set; }
        [JsonProperty("clientRole")]
        public bool ClientRole { get; set; }
        [JsonProperty("containerId")]
        public string ContainerId { get; set; }
    }
}
