using KeycloakStandard.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KeycloakStandard
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
        /// Try to login with provided username and password. If username or password are invalide, this method will return new empty KeycloakToken object.
        /// </summary>
        /// <param name="username">Username of user that needs to be authenticated.</param>
        /// <param name="password">Password of user that needs to be authenticated.</param>
        /// <returns></returns>
        public async Task<KeycloakToken> Login(string username, string password)
        {
            StringBuilder data = new StringBuilder();

            data.Append($"client_id={_clientData.ClientId}&");
            data.Append($"client_secret={_clientData.ClientSecret}&");
            data.Append($"username={username}&");
            data.Append($"password={password}&");
            data.Append($"grant_type=password&");

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(data.ToString()))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await httpClient.PostAsync(_clientData.BaseUrl + KeycloakConstants.loginEndpoint, httpContent);

                    string json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<KeycloakToken>(json);
                }
            }
        }

        /// <summary>
        /// Register new user with filled Registration object.
        /// </summary>
        /// <param name="userRegistration">Instance of Registration object with filled data.</param>
        /// <returns></returns>
        public async Task<KeycloakToken> Registration(Registration userRegistration)
        {
            KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

            StringBuilder data = new StringBuilder();

            data.Append("{");
            data.Append($"\"email\": \"{userRegistration.Email}\",");
            data.Append($"\"username\": \"{userRegistration.Username}\",");
            data.Append($"\"firstName\": \"{userRegistration.FirstName}\",");
            data.Append($"\"lastName\": \"{userRegistration.LastName}\",");
            data.Append($"\"enabled\": {userRegistration.Enabled},");
            data.Append($"\"emailVerified\": {userRegistration.EmailVerified}");
            data.Append("}");

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(data.ToString()))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.PostAsync(KeycloakConstants.userEndpoint, httpContent);

                    string[] locationSegments = response.Headers.Location.AbsoluteUri.Split('/');

                    string userGuid = locationSegments[locationSegments.Length - 1];

                    StringBuilder passwordData = new StringBuilder();

                    passwordData.Append("{");
                    passwordData.Append($"\"temporary\": {userRegistration.Temporary},");
                    passwordData.Append($"\"type\": \"password\",");
                    passwordData.Append($"\"value\": \"{userRegistration.Password}\"");
                    passwordData.Append("}");

                    using (HttpClient resetPasswordClient = new HttpClient())
                    {
                        using (HttpContent resetPasswordContent = new StringContent(passwordData.ToString()))
                        {
                            resetPasswordClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                            resetPasswordContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var resetUrl = _clientData.BaseUrl + KeycloakConstants.userEndpoint + userGuid + "/reset-password";

                            var response2 = await resetPasswordClient.PutAsync(resetUrl, resetPasswordContent);

                            var login = await Login(userRegistration.Username, userRegistration.Password);

                            return (response2.StatusCode.Equals(HttpStatusCode.NoContent)) ? login : new KeycloakToken();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Logout user which information is filled in Logout object.
        /// </summary>
        /// <param name="logout">Instance of Logout object with filled data.</param>
        /// <returns></returns>
        public async Task<bool> Logout(Logout logout)
        {
            StringBuilder data = new StringBuilder();

            data.Append($"client_id={_clientData.ClientId}&");
            data.Append($"client_secret={_clientData.ClientSecret}&");
            data.Append($"refresh_token={logout.RefreshToken}");

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(data.ToString()))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", logout.AccessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await httpClient.PostAsync(KeycloakConstants.logoutEndpoint, httpContent);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
        }

        /// <summary>
        /// Delete user which information is filled in DeleteUser object.
        /// </summary>
        /// <param name="logout">Instance of DeleteUser object with filled data.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(DeleteUser<TUserIdType> deleteUser)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deleteUser.AccessToken);

                var response = await httpClient.DeleteAsync(_clientData.BaseUrl + KeycloakConstants.userEndpoint + deleteUser.UserGuid);

                return response.StatusCode.Equals(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Create new Keycloak client with filled KeycloakClient information and with access token of account that have rights to create new client.
        /// </summary>
        /// <param name="keycloakClient">Instance of KeycloakClient object with filled data.</param>
        /// <param name="accessToken">Access token of account that have rights to create new client.</param>
        /// <returns></returns>
        public async Task<bool> CreateClient(KeycloakClient keycloakClient, string accessToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(keycloakClient)))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.PostAsync(KeycloakConstants.clientEndpoint, httpContent);
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
        public async Task<ICollection<KeycloakClient>> GetAllClients(string accessToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(accessToken)))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.GetAsync(KeycloakConstants.clientEndpoint);
                    
                    if(response.StatusCode.Equals(HttpStatusCode.OK))
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
        public async Task<bool> DeleteClient(string clientGuid, string accessToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.DeleteAsync(KeycloakConstants.clientEndpoint + "/" + clientGuid);

                return response.StatusCode.Equals(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Update existing client with filled instance of KeycloakClient object and valid client guid. 
        /// </summary>
        /// <param name="keycloakClient">Filled instance of KeycloakClient object.</param>
        /// <param name="clientGuid">Valid client guid.</param>
        /// <returns></returns>
        public async Task<bool> UpdateClient(KeycloakClient keycloakClient, string accessToken, string clientGuid)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(keycloakClient)))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.PutAsync(KeycloakConstants.clientEndpoint + "/" + clientGuid, httpContent);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
        }
    }
}
