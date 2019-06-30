namespace KeycloakStandard.Endpoints
{
    /// <summary>
    /// Keycloak client endpoints
    /// </summary>
    public static class ClientEndpoints
    {
        public static string ClientEndpoint(string realm) => $"/auth/admin/realms/{realm}/clients";
    }
}
