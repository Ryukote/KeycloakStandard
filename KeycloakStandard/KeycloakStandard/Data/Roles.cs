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
    public class Roles
    {
        private ClientData _clientData = new ClientData();

        /// <summary>
        /// Roles constructor.
        /// </summary>
        /// <param name="clientData">Instance of ClientData object with filled data.</param>
        public Roles(ClientData clientData)
        {
            _clientData = clientData;
        }

        public async Task<ICollection<RealmRole>> GetAllRealmRoles(string accessToken, string realm)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.GetAsync(_clientData.BaseUrl + RoleEndpoints.RealmRoles(realm));

                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ICollection<RealmRole>>(json);
            }
        }

        public async Task<bool> CreateRealmRole(string accessToken, string realm, RealmRole role)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (HttpContent content = new StringContent(JsonConvert.SerializeObject(role)))
                {
                    var response = await httpClient.PostAsync(_clientData.BaseUrl + RoleEndpoints.RealmRoles(realm), content);

                    return response.StatusCode.Equals(HttpStatusCode.Created);
                }
            }
        }

        public async Task<bool> UpdateRealmRole(string accessToken, string realm, RealmRole role)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (HttpContent content = new StringContent(JsonConvert.SerializeObject(role)))
                {
                    var response = await httpClient.PutAsync(_clientData.BaseUrl + RoleEndpoints.UpdateOrDeleteRealmRole(realm, role.Id), content);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
        }

        public async Task<bool> DeleteRealmRole(string accessToken, string realm, RealmRole role)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.DeleteAsync(_clientData.BaseUrl + RoleEndpoints.UpdateOrDeleteRealmRole(realm, role.Id));

                return response.StatusCode.Equals(HttpStatusCode.NoContent);
            }
        }
    }
}
