using KeycloakStandard.Contracts;
using KeycloakStandard.Models;
using KeycloakStandard.Models.Base;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Text;

namespace KeycloakStandard.Service
{
    public class RealmService : IRealmService
    {
        private readonly ClientData _clientData;

        /// <summary>
        /// RealmService constructor.
        /// </summary>
        /// <param name="clientData">Instance of ClientData object with filled data.</param>
        public RealmService(ClientData clientData)
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

        public async Task AssignRealmRolesToUser(RoleRepresentation data, string userId)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    var list = new List<RoleRepresentation>();
                    list.Add(data);

                    using (HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(list)))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.PostAsync(_clientData.BaseUrl + $"{KeycloakEndpoints.AssignRealmRoleToUserEndpoint(_clientData.RealmName, userId)}", httpContent);

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
