using Newtonsoft.Json;
using System.Collections.Generic;

namespace KeycloakStandard.Models
{
    public class UserEnvironmentRole
    {
        [JsonProperty("OrganizationId")]
        public string OrganizationId { get; set; }

        [JsonProperty("RoleIds")]
        public List<int> RoleIds { get; set; }
    }
}
