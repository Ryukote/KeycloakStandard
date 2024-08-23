using Newtonsoft.Json;
using System;

namespace KeycloakStandard.Models
{
    public class UserDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("createdTimestamp")]
        public Int64 CreatedTimestamp { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("totp")]
        public bool Totp { get; set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("attributes")]
        public AttributesJson Attributes { get; set; }
        [JsonProperty("notBefore")]
        public int NotBefore { get; set; }
    }
}
