namespace KeycloakStandard
{
    /// <summary>
    /// Standard Keycloak endpoints
    /// </summary>
    public static class KeycloakConstants
    {
        public const string loginEndpoint = "auth/realms/master/protocol/openid-connect/token";
        public const string registerEndpoint = "auth/admin/realms/master/users";
        public const string resetPasswordEndpoint = "auth/admin/realms/master/users/";
        public const string logoutEndpoint = "auth/realms/master/protocol/openid-connect/logout";
        public const string deleteUserEndpoint = "auth/admin/realms/master/users/";
    }
}
