using Newtonsoft.Json;

namespace KeycloakStandard.Models
{
    /// <summary>
    /// Token information from Keycloak.
    /// </summary>
    public class KeycloakToken
    {
        /// <summary>
        /// Token used for authenticating and authorizing on other API calls.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// When token is expiring.
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        /// <summary>
        /// When refresh token is expiring.
        /// </summary>
        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }
        /// <summary>
        /// Refresh token.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// Token type (default is Bearer).
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        /// <summary>
        /// Policy for when should refresh occure.
        /// </summary>
        [JsonProperty("not-before-policy")]
        public int NotBeforePolicy { get; set; }
        /// <summary>
        /// State of session.
        /// </summary>
        [JsonProperty("session_state")]
        public string SessionState { get; set; }
        /// <summary>
        /// List of scopes delimited with whitespace.
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
