namespace KeycloakNet10;

/// <summary>
/// Standard Keycloak endpoints.
/// </summary>
public static class KeycloakConstants
{
    public const string LoginEndpoint = "realms/master/protocol/openid-connect/token";
    public const string UserEndpoint = "admin/realms/master/users";
    public const string LogoutEndpoint = "realms/master/protocol/openid-connect/logout";
    public const string ClientEndpoint = "admin/realms/master/clients";
}
