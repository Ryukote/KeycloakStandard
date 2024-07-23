namespace KeycloakStandard
{
    /// <summary>
    /// Standard Keycloak endpoints
    /// </summary>
    public static class KeycloakEndpoints
    {
        public static string LoginEndpoint(string realmName) =>  $"realms/{realmName}/protocol/openid-connect/token";

        public static string UserEndpoint(string realmName) => $"admin/realms/{realmName}/users";

        public static string LogoutEndpoint(string realmName) => $"realms/{realmName}/protocol/openid-connect/logout";

        public static string ClientEndpoint(string realmName) => $"admin/realms/${realmName}/clients";
        public static string ClientRolesForUserEndpoint(string realmName, string clientId, string userId) => $"admin/realms/{realmName}/users/{userId}/role-mappings/clients/{clientId}";
        public static string ResetPasswordEndpoint(string realmName, string userId) => $"admin/realms/{realmName}/users/{userId}/reset-password";
        public static string AssignRealmRoleToUserEndpoint(string realmName, string userId) => $"admin/realms/{realmName}/users/{userId}/role-mappings/realm";
        public static string UserInfoEndpoint(string realmName, string userId) => $"admin/realms/{realmName}/users/{userId}";
    }
}
