using KeycloakStandard.Models.Base;
using KeycloakStandard.Models.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakStandard.Contracts
{
    public interface IClientService
    {
        /// <summary>
        /// Try to login with provided username and password. If username or password are invalide, this method will return new empty KeycloakToken object.
        /// </summary>
        /// <param name="username">Username of user that needs to be authenticated.</param>
        /// <param name="password">Password of user that needs to be authenticated.</param>
        /// <returns></returns>
        Task<KeycloakToken> Login(string username, string password);

        /// <summary>
        /// Create new Keycloak client with filled KeycloakClient information and with access token of account that have rights to create new client.
        /// </summary>
        /// <param name="keycloakClient">Instance of KeycloakClient object with filled data.</param>
        /// <param name="accessToken">Access token of account that have rights to create new client.</param>
        /// <returns></returns>
        Task<bool> CreateClient(KeycloakClient keycloakClient, string accessToken);
        /// <summary>
        /// Get all clients under specific realm.
        /// </summary>
        /// <param name="keycloakClient">Instance of KeycloakClient object with filled data.</param>
        /// <param name="accessToken">Access token of account that have rights to create new client.</param>
        /// <returns></returns>
        Task<ICollection<KeycloakClient>> GetAllClients(string accessToken);
        /// <summary>
        /// Delete existing client with provided guid of client and access token of account that have rights to delete client.
        /// </summary>
        /// <param name="clientGuid">Validi client guid.</param>
        /// <param name="accessToken">Access token of account that have rights to delete client.</param>
        /// <returns></returns>
        Task<bool> DeleteClient(string clientGuid, string accessToken);
        /// <summary>
        /// Update existing client with filled instance of KeycloakClient object and valid client guid. 
        /// </summary>
        /// <param name="keycloakClient">Filled instance of KeycloakClient object.</param>
        /// <param name="clientGuid">Valid client guid.</param>
        /// <returns></returns>
        Task<bool> UpdateClient(KeycloakClient keycloakClient, string accessToken, string clientGuid);

        Task<bool> AssignClientRolesToUserAsync(Guid userId, ICollection<ClientRole> roles);
    }
}
