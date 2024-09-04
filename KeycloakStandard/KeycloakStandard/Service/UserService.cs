using KeycloakStandard.Contracts;
using KeycloakStandard.Models;
using KeycloakStandard.Models.Base;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using KeycloakStandard.Models.User;
using KeycloakStandard.Exceptions;
using System.Collections.Generic;
using System.Net;

namespace KeycloakStandard.Service
{
    public class UserService : IUserService
    {
        private readonly ClientData _clientData;

        /// <summary>
        /// UserClient constructor.
        /// </summary>
        /// <param name="clientData">Instance of ClientData object with filled data.</param>
        public UserService(ClientData clientData)
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
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<string> Impersonation(string userId)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpContent emptyContent = new StringContent(string.Empty))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        emptyContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await httpClient.PostAsync(_clientData.BaseUrl + KeycloakEndpoints.UserImpersonationEndpoint(_clientData.RealmName, userId), emptyContent);

                        var content = response?.Content?.ReadAsStringAsync();

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

                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            throw new UserAlreadyExistException(await response?.Content?.ReadAsStringAsync());
                        }

                        var content = response?.Content?.ReadAsStringAsync();

                        string[] locationSegments = response.Headers.Location.AbsoluteUri.Split('/');

                        return locationSegments[locationSegments.Length - 1];
                    }
                }
            }
            catch (UserAlreadyExistException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Logout(Logout logout)
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteUser(DeleteUser deleteUser)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deleteUser.AccessToken);

                    var response = await httpClient.DeleteAsync(_clientData.BaseUrl + KeycloakEndpoints.UserEndpoint(_clientData.RealmName) + deleteUser.UserId);

                    return response.StatusCode.Equals(HttpStatusCode.NoContent);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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

                        var content = response?.Content?.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ICollection<UserDetails>> GetAllUsersAsync()
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    var response = await httpClient.GetAsync(_clientData.BaseUrl + $"{KeycloakEndpoints.UserEndpoint(_clientData.RealmName)}" + "?username=sowegef911@segichen.com");

                    var result = await response?.Content?.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<ICollection<UserDetails>>(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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

        public async Task<dynamic> GetUserInfo(string keycloakUserId)
        {
            try
            {
                KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    var response = await httpClient.GetAsync(_clientData.BaseUrl + KeycloakEndpoints.UserInfoEndpoint(_clientData.RealmName, keycloakUserId));

                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<dynamic>(data);
                    }

                    if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        throw new Exception();
                    }

                    else if (response.StatusCode.Equals(HttpStatusCode.Forbidden))
                    {
                        throw new Exception();
                    }

                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
