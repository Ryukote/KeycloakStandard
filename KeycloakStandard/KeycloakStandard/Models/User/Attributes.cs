using Newtonsoft.Json;
using System.Collections.Generic;

namespace KeycloakStandard.Models.User
{
    public class Attributes
    {
        [JsonProperty("OrganizationId")]
        public dynamic OrganizationId { get; set; }
        [JsonProperty("UserEnvironmentRoles")]
        public List<dynamic> UserEnvironmentRoles { get; set; }
    }
}
