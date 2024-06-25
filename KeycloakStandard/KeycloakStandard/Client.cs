using KeycloakStandard.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KeycloakStandard
{
    public class Client
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

            data.Append($"username=${username}&");
            data.Append($"password=${password}&");

            data.Append($"client_id={_clientData.AdminClientId}&");
            data.Append($"client_secret={_clientData.AdminClientSecret}&");
            data.Append($"grant_type=client_credentials");

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpContent httpContent = new StringContent(data.ToString()))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await httpClient.PostAsync(_clientData.BaseUrl + KeycloakEndpoints.LoginEndpoint(_clientData.RealmName), httpContent);

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
        public async Task<string> Registration(Registration userRegistration)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                StringBuilder data = new StringBuilder();

                var collection = new List<Credentials>
                {
                    new Credentials()
                    {
                        Temporary = userRegistration.Temporary,
                        Type = userRegistration.CredentialType,
                        Value = userRegistration.Password
                    }
                };

                //var attributes = new Attributes();

                var newUser = new CreateUser()
                {
                    Credentials = collection,
                    Email = userRegistration.Email,
                    EmailVerified = userRegistration.EmailVerified,
                    Enabled = userRegistration.Enabled,
                    FirstName = userRegistration.FirstName,
                    LastName = userRegistration.LastName,
                    Username = userRegistration.Username,
                    Attributes = userRegistration.Attributes
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(newUser)))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.PostAsync(_clientData.BaseUrl + KeycloakEndpoints.UserEndpoint(_clientData.RealmName), httpContent);

                        var a = response?.Content?.ReadAsStringAsync();

                        string[] locationSegments = response.Headers.Location.AbsoluteUri.Split('/');

                        return locationSegments[locationSegments.Length - 1];
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
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

                    var response = await httpClient.PostAsync(KeycloakEndpoints.LogoutEndpoint(_clientData.RealmName), httpContent);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
        }

        /// <summary>
        /// Delete user which information is filled in DeleteUser object.
        /// </summary>
        /// <param name="logout">Instance of DeleteUser object with filled data.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(DeleteUser deleteUser)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deleteUser.AccessToken);

                var response = await httpClient.DeleteAsync(_clientData.BaseUrl + KeycloakEndpoints.UserEndpoint(_clientData.RealmName) + deleteUser.UserId);

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

                    var response = await httpClient.PostAsync(KeycloakEndpoints.ClientEndpoint(_clientData.RealmName), httpContent);
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

                    var response = await httpClient.GetAsync(KeycloakEndpoints.ClientEndpoint(_clientData.RealmName));

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
        public async Task<bool> DeleteClient(string clientGuid, string accessToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.DeleteAsync(KeycloakEndpoints.ClientEndpoint(_clientData.RealmName) + "/" + clientGuid);

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

                    var response = await httpClient.PutAsync(KeycloakEndpoints.ClientEndpoint(_clientData.RealmName) + "/" + clientGuid, httpContent);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
        }

        /// <summary>
        /// Register new user with filled Registration object.
        /// </summary>
        /// <param name="userRegistration">Instance of Registration object with filled data.</param>
        /// <returns></returns>
        public async Task UpdateUser(UpdateUser updateUser, string userId)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                var updatedUser = new UpdateUser()
                {
                    Id = userId,
                    Attributes = updateUser.Attributes
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(updatedUser)))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.PutAsync(_clientData.BaseUrl + $"{KeycloakEndpoints.UserEndpoint(_clientData.RealmName)}/${userId}", httpContent);

                        var a = response?.Content?.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GetAllUsersAsync()
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    var response = await httpClient.GetAsync(_clientData.BaseUrl + $"{KeycloakEndpoints.UserEndpoint(_clientData.RealmName)}");

                    return await response?.Content?.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AssignClientRolesToUserAsync(Guid userId, ICollection<ClientRole> roles)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(roles)))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.PostAsync(_clientData.BaseUrl + $"{KeycloakEndpoints.ClientRolesForUserEndpoint(_clientData.RealmName, _clientData.ClientId, userId.ToString())}", httpContent);

                        return response.StatusCode.Equals(HttpStatusCode.NoContent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Reset password for provided Keycloak user id.
        /// </summary>
        /// <param name="userId">User id that can be found in user details in Keycloak.</param>
        /// <returns></returns>
        public async Task ResetPassword(string userId, ResetPassword resetPassword)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(resetPassword)))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.PutAsync(_clientData.BaseUrl + $"{KeycloakEndpoints.ResetPasswordEndpoint(_clientData.RealmName, userId)}", httpContent);

                        var a = response?.Content?.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
