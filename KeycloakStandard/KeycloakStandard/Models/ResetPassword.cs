namespace KeycloakStandard.Models
{
    public class ResetPassword
    {
        public string Type { get; set; }
        public bool Temporary { get; set; }
        public string Value { get; set; }
    }
}
