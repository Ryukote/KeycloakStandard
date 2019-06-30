namespace KeycloakStandard.Endpoints
{
    /// <summary>
    /// Keycloak user endpoints
    /// </summary>
    public static class UserEndpoints
    {
        public static string LoginEndpoint(string realm) => $"/auth/realms/{realm}/protocol/openid-connect/token";
        public static string UserEndpoint(string realm) => $"/auth/admin/realms/{realm}/users";
        public static string LogoutEndpoint(string realm) => $"/auth/realms/{realm}/protocol/openid-connect/logout";
    }
}
