using KeycloakStandard.Models;
using KeycloakStandard.Models.Base;
using System.Threading.Tasks;

namespace KeycloakStandard.Contracts
{
    public interface IRealmService
    {
        /// <summary>
        /// Try to login with provided username and password. If username or password are invalide, this method will return new empty KeycloakToken object.
        /// </summary>
        /// <param name="username">Username of user that needs to be authenticated.</param>
        /// <param name="password">Password of user that needs to be authenticated.</param>
        /// <returns></returns>
        Task<KeycloakToken> Login(string username, string password);
        Task AssignRealmRolesToUser(RoleRepresentation data, string userId);
    }
}
