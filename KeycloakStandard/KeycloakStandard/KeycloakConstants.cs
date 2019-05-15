namespace KeycloakStandard
{
    /// <summary>
    /// Standard Keycloak endpoints
    /// </summary>
    public static class KeycloakConstants
    {
        public const string loginEndpoint = "auth/realms/master/protocol/openid-connect/token";
        public const string userEndpoint = "auth/admin/realms/master/users";
        public const string logoutEndpoint = "auth/realms/master/protocol/openid-connect/logout";
        public const string clientEndpoint = "auth/admin/realms/master/clients";
    }
}
