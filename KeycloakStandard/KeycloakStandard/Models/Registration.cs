namespace KeycloakStandard.Models
{
    /// <summary>
    /// Data for registering a new user.
    /// </summary>
    public class Registration
    {
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public bool Enabled { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Temporary { get; set; }
    }
}
