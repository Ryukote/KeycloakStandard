namespace KeycloakStandard.Models
{
    /// <summary>
    /// Data for deleting user.
    /// </summary>
    public class DeleteUser
    {
        public string AccessToken { get; set; }
        public string UserId { get; set; }
    }
}
