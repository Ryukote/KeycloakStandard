using KeycloakStandard.Models;
using KeycloakStandard.Models.Base;
using KeycloakStandard.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakStandard.Contracts
{
    public interface IUserService
    {
        /// <summary>
        /// Try to login with provided username and password. If username or password are invalide, this method will return new empty KeycloakToken object.
        /// </summary>
        /// <param name="username">Username of user that needs to be authenticated.</param>
        /// <param name="password">Password of user that needs to be authenticated.</param>
        /// <returns></returns>
        Task<KeycloakToken> Login(string username, string password);
        /// <summary>
        /// Impersonate the user with userId which can be found under user details in Keycloak.
        /// </summary>
        /// <param name="userId">Id of a user registered in Keycloak realm.</param>
        /// <returns></returns>
        Task<string> Impersonation(string userId);
        /// <summary>
        /// Register new user with filled Registration object.
        /// </summary>
        /// <param name="userRegistration">Instance of Registration object with filled data.</param>
        /// <returns></returns>
        Task<string> Registration(Registration userRegistration);
        /// <summary>
        /// Logout user which information is filled in Logout object.
        /// </summary>
        /// <param name="logout">Instance of Logout object with filled data.</param>
        /// <returns></returns>
        Task<bool> Logout(Logout logout);
        /// <summary>
        /// Delete user which information is filled in DeleteUser object.
        /// </summary>
        /// <param name="logout">Instance of DeleteUser object with filled data.</param>
        /// <returns></returns>
        Task<bool> DeleteUser(DeleteUser deleteUser);
        /// <summary>
        /// Update existing user with filled UpdateUser object and userId.
        /// </summary>
        /// <param name="updateUser">Instance of UpdateUser object with filled data.</param>
        /// <param name="userId">Id of user registered in Keycloak under specific realm.</param>
        /// <returns></returns>
        Task UpdateUser(UpdateUser updateUser, string userId);
        /// <summary>
        /// Get all users in specific realm provided in constructor object.
        /// </summary>
        /// <returns>List of all registered users under realm provided in constructor object.</returns>
        Task<ICollection<UserDetails>> GetAllUsersAsync();
        /// <summary>
        /// Reset password for provided Keycloak user id.
        /// </summary>
        /// <param name="userId">User id that can be found in user details in Keycloak.</param>
        /// <returns></returns>
        Task ResetPassword(string userId, ResetPassword resetPassword);
        /// <summary>
        /// Get user information.
        /// </summary>
        /// <param name="keycloakUserId">User Id that can be found in Keycloak when viewing user in the realm.</param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(string keycloakUserId);
    }
}
