namespace KeycloakStandard.Models
{
    /// <summary>
    /// Data for logging user out.
    /// </summary>
    public class Logout
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
