namespace KeycloakStandard.Models
{
    /// <summary>
    /// Data for deleting user.
    /// </summary>
    public class DeleteUser<TId> where TId : struct
    {
        public string AccessToken { get; set; }
        public TId UserGuid { get; set; }
    }
}
