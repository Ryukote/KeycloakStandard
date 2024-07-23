using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class UserInfo
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
