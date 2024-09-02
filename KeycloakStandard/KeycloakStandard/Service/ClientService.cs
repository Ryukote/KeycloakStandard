using KeycloakStandard.Contracts;
using KeycloakStandard.Models;
using KeycloakStandard.Models.Base;
using KeycloakStandard.Models.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KeycloakStandard.Service
{
    public class ClientService : IClientService
    {
        private readonly ClientData _clientData;

        /// <summary>
        /// UserClient constructor.
        /// </summary>
        /// <param name="clientData">Instance of ClientData object with filled data.</param>
        public ClientService(ClientData clientData)
        {
            _clientData = clientData;
        }

        public async Task<KeycloakToken> Login(string username, string password)
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CreateClient(KeycloakClient keycloakClient, string accessToken)
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ICollection<KeycloakClient>> GetAllClients(string accessToken)
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteClient(string clientGuid, string accessToken)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await httpClient.DeleteAsync(KeycloakEndpoints.ClientEndpoint(_clientData.RealmName) + "/" + clientGuid);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateClient(KeycloakClient keycloakClient, string accessToken, string clientGuid)
        {
            try
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
    }
}
