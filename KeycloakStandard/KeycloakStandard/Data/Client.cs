using KeycloakStandard.Endpoints;
using KeycloakStandard.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KeycloakStandard.Data
{
    public class Client<TUserIdType> where TUserIdType : struct
    {
        private ClientData _clientData = new ClientData();

        /// <summary>
        /// Client constructor.
        /// </summary>
        /// <param name="clientData">Instance of ClientData object with filled data.</param>
        public Client(ClientData clientData)
        {
            _clientData = clientData;
        }

        /// <summary>
        /// Create new Keycloak client with filled KeycloakClient information and with access token of account that have rights to create new client.
        /// </summary>
        /// <param name="keycloakClient">Instance of KeycloakClient object with filled data.</param>
        /// <param name="accessToken">Access token of account that have rights to create new client.</param>
        /// <returns></returns>
        public async Task<bool> CreateClient(KeycloakClient keycloakClient, string accessToken, string realmName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(keycloakClient)))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.PostAsync(ClientEndpoints.ClientEndpoint(realmName), httpContent);
                    return response.StatusCode.Equals(HttpStatusCode.Created);
                }
            }
        }

        /// <summary>
        /// Create new Keycloak client with filled KeycloakClient information and with access token of account that have rights to create new client.
        /// </summary>
        /// <param name="keycloakClient">Instance of KeycloakClient object with filled data.</param>
        /// <param name="accessToken">Access token of account that have rights to create new client.</param>
        /// <returns></returns>
        public async Task<ICollection<KeycloakClient>> GetAllClients(string accessToken, string realmName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(accessToken))) 
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.GetAsync(ClientEndpoints.ClientEndpoint(realmName));

                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        return JsonConvert.DeserializeObject<ICollection<KeycloakClient>>(await response.Content.ReadAsStringAsync());
                    }

                    return new List<KeycloakClient>();
                }
            }
        }

        /// <summary>
        /// Delete existing client with provided guid of client and access token of account that have rights to delete client.
        /// </summary>
        /// <param name="clientGuid">Validi client guid.</param>
        /// <param name="accessToken">Access token of account that have rights to delete client.</param>
        /// <returns></returns>
        public async Task<bool> DeleteClient(string clientGuid, string accessToken, string realmName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.DeleteAsync(ClientEndpoints.ClientEndpoint(realmName) + "/" + clientGuid);

                return response.StatusCode.Equals(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Update existing client with filled instance of KeycloakClient object and valid client guid. 
        /// </summary>
        /// <param name="keycloakClient">Filled instance of KeycloakClient object.</param>
        /// <param name="clientGuid">Valid client guid.</param>
        /// <returns></returns>
        public async Task<bool> UpdateClient(KeycloakClient keycloakClient, string accessToken, string clientGuid, string realmName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(keycloakClient)))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.PutAsync(ClientEndpoints.ClientEndpoint(realmName) + "/" + clientGuid, httpContent);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
        }
    }
}
