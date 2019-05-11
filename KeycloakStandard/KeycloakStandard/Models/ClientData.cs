namespace KeycloakStandard.Models
{
    /// <summary>
    /// Data for communication with Keycloak.
    /// </summary>
    public class ClientData
    {
        public string BaseUrl { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
