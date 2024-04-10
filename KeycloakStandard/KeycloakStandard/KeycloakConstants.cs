using System.Diagnostics.CodeAnalysis;

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
    }
}
