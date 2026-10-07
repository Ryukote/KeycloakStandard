using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using KeycloakNet10.Models;

namespace KeycloakNet10;

public class Client<TUserIdType> where TUserIdType : struct
{
    private readonly ClientData _clientData;
    private readonly HttpClient _httpClient;

    /// <param name="clientData">Instance of ClientData with filled data.</param>
    /// <param name="httpClient">Optional shared HttpClient (recommended, e.g. from IHttpClientFactory).</param>
    public Client(ClientData clientData, HttpClient? httpClient = null)
    {
        _clientData = clientData ?? throw new ArgumentNullException(nameof(clientData));
        _httpClient = httpClient ?? new HttpClient();
    }

    private string Url(string endpoint) => _clientData.BaseUrl + endpoint;

    private static HttpRequestMessage Request(HttpMethod method, string url, string? accessToken = null, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, url) { Content = content };
        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        return request;
    }

    /// <summary>
    /// Try to login. If credentials are invalid, returns a token object without values.
    /// </summary>
    public async Task<KeycloakToken> Login(string username, string password)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _clientData.ClientId,
            ["client_secret"] = _clientData.ClientSecret,
            ["username"] = username,
            ["password"] = password,
            ["grant_type"] = "password"
        });

        using var request = Request(HttpMethod.Post, Url(KeycloakConstants.LoginEndpoint), content: content);
        using var response = await _httpClient.SendAsync(request);

        try
        {
            return await response.Content.ReadFromJsonAsync<KeycloakToken>() ?? new KeycloakToken();
        }
        catch (System.Text.Json.JsonException)
        {
            return new KeycloakToken();
        }
    }

    /// <summary>
    /// Register a new user and return a login token for it.
    /// </summary>
    public async Task<KeycloakToken> Registration(Registration userRegistration)
    {
        KeycloakToken token = await Login(_clientData.AdminUsername, _clientData.AdminPassword);

        using var userContent = JsonContent.Create(new
        {
            email = userRegistration.Email,
            username = userRegistration.Username,
            firstName = userRegistration.FirstName,
            lastName = userRegistration.LastName,
            enabled = userRegistration.Enabled,
            emailVerified = userRegistration.EmailVerified
        });

        using var createRequest = Request(HttpMethod.Post, Url(KeycloakConstants.UserEndpoint), token.AccessToken, userContent);
        using var createResponse = await _httpClient.SendAsync(createRequest);

        var location = createResponse.Headers.Location;
        if (location == null)
        {
            return new KeycloakToken();
        }

        string userGuid = location.AbsoluteUri.Split('/').Last();

        using var passwordContent = JsonContent.Create(new
        {
            temporary = userRegistration.Temporary,
            type = "password",
            value = userRegistration.Password
        });

        using var resetRequest = Request(HttpMethod.Put,
            Url(KeycloakConstants.UserEndpoint) + "/" + userGuid + "/reset-password", token.AccessToken, passwordContent);
        using var resetResponse = await _httpClient.SendAsync(resetRequest);

        if (resetResponse.StatusCode != HttpStatusCode.NoContent)
        {
            return new KeycloakToken();
        }

        return await Login(userRegistration.Username, userRegistration.Password);
    }

    /// <summary>
    /// Logout user.
    /// </summary>
    public async Task<bool> Logout(Logout logout)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _clientData.ClientId,
            ["client_secret"] = _clientData.ClientSecret,
            ["refresh_token"] = logout.RefreshToken
        });

        using var request = Request(HttpMethod.Post, Url(KeycloakConstants.LogoutEndpoint), logout.AccessToken, content);
        using var response = await _httpClient.SendAsync(request);
        return response.StatusCode == HttpStatusCode.NoContent;
    }

    /// <summary>
    /// Delete user.
    /// </summary>
    public async Task<bool> DeleteUser(DeleteUser<TUserIdType> deleteUser)
    {
        using var request = Request(HttpMethod.Delete,
            Url(KeycloakConstants.UserEndpoint) + "/" + deleteUser.UserGuid, deleteUser.AccessToken);
        using var response = await _httpClient.SendAsync(request);
        return response.StatusCode == HttpStatusCode.NoContent;
    }

    /// <summary>
    /// Create a new Keycloak client.
    /// </summary>
    public async Task<bool> CreateClient(KeycloakClient keycloakClient, string accessToken)
    {
        using var content = JsonContent.Create(keycloakClient);
        using var request = Request(HttpMethod.Post, Url(KeycloakConstants.ClientEndpoint), accessToken, content);
        using var response = await _httpClient.SendAsync(request);
        return response.StatusCode == HttpStatusCode.Created;
    }

    /// <summary>
    /// Get all Keycloak clients.
    /// </summary>
    public async Task<ICollection<KeycloakClient>> GetAllClients(string accessToken)
    {
        using var request = Request(HttpMethod.Get, Url(KeycloakConstants.ClientEndpoint), accessToken);
        using var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.Content.ReadFromJsonAsync<List<KeycloakClient>>() ?? new List<KeycloakClient>();
        }

        return new List<KeycloakClient>();
    }

    /// <summary>
    /// Delete an existing Keycloak client.
    /// </summary>
    public async Task<bool> DeleteClient(string clientGuid, string accessToken)
    {
        using var request = Request(HttpMethod.Delete,
            Url(KeycloakConstants.ClientEndpoint) + "/" + clientGuid, accessToken);
        using var response = await _httpClient.SendAsync(request);
        return response.StatusCode == HttpStatusCode.NoContent;
    }

    /// <summary>
    /// Update an existing Keycloak client.
    /// </summary>
    public async Task<bool> UpdateClient(KeycloakClient keycloakClient, string accessToken, string clientGuid)
    {
        using var content = JsonContent.Create(keycloakClient);
        using var request = Request(HttpMethod.Put,
            Url(KeycloakConstants.ClientEndpoint) + "/" + clientGuid, accessToken, content);
        using var response = await _httpClient.SendAsync(request);
        return response.StatusCode == HttpStatusCode.NoContent;
    }
}
