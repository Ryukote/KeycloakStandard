using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    public class UpdateUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }
        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }
    }
}
