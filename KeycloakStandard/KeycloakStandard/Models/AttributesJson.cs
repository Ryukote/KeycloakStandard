using Newtonsoft.Json;
using System.Collections.Generic;

namespace KeycloakStandard.Models
{
    public class AttributesJson
    {
        [JsonProperty("OrganizationId")]
        public string OrganizationId { get; set; }

        [JsonProperty("UserEnvironmentRoles")]
        public List<string> UserEnvironmentRolesRaw { get; set; }

        [JsonIgnore]
        public List<UserEnvironmentRole> UserEnvironmentRoles
        {
            get
            {
                //if (UserEnvironmentRolesRaw == null || UserEnvironmentRolesRaw.Count == 0)
                //    return null;

                var roles = new List<UserEnvironmentRole>();

                foreach (var rawRole in UserEnvironmentRolesRaw)
                {
                    roles.Add(JsonConvert.DeserializeObject<UserEnvironmentRole>(rawRole));
                }

                return roles;
            }
        }
    }
}
