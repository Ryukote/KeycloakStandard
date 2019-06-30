namespace KeycloakStandard.Endpoints
{
    public static class RoleEndpoints
    {
        public static string RealmRoles(string realm) => $"/auth/admin/realms/{realm}/roles";

        public static string UpdateOrDeleteRealmRole(string realm, string roleId) => $"/auth/admin/realms/{realm}/roles-by-id/{roleId}";
    }
}
