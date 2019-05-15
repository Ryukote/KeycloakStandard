using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace KeycloakStandard.Models
{
    /// <summary>
    /// Data for Keycloak client.
    /// </summary>
    public class KeycloakClient
    {
        [JsonProperty("attributes")]
        public JArray Attributes { get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = default;
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("redirectUris")]
        public JObject RedirectUris { get; set; }
        [JsonProperty("rootUrl")]
        public string RootUrl { get; set; }
    }
}
