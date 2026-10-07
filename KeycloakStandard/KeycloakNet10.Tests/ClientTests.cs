using KeycloakNet10;
using KeycloakNet10.Models;
using System.Buffers.Text;
using System.Net;
using System.Text;

namespace KeycloakNet10.Tests;

public class ClientTests
{
    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, string, HttpResponseMessage> _responder;
        public List<(HttpRequestMessage Request, string Body)> Calls { get; } = new();

        public FakeHandler(Func<HttpRequestMessage, string, HttpResponseMessage> responder) => _responder = responder;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string body = request.Content == null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);
            Calls.Add((request, body));
            return _responder(request, body);
        }
    }

    private static readonly ClientData Data = new()
    {
        BaseUrl = "http://localhost:8080/",
        ClientId = "cid",
        ClientSecret = "secret",
        AdminUsername = "admin",
        AdminPassword = "adminpw"
    };

    private static HttpResponseMessage Json(string json, HttpStatusCode code = HttpStatusCode.OK) =>
        new(code) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    [Fact]
    public async Task Login_SendsEncodedFormAndParsesToken()
    {
        var handler = new FakeHandler((_, _) => Json("{\"access_token\":\"abc\",\"refresh_token\":\"ref\",\"expires_in\":60}"));
        var client = new Client<Guid>(Data, new HttpClient(handler));

        var token = await client.Login("user", "p&ss+word");

        Assert.Equal("abc", token.AccessToken);
        Assert.Equal("ref", token.RefreshToken);
        Assert.Equal(60, token.ExpiresIn);

        var call = handler.Calls.Single();
        Assert.Equal(Data.BaseUrl + KeycloakConstants.LoginEndpoint, call.Request.RequestUri!.ToString());
        Assert.Contains("password=p%26ss%2Bword", call.Body);
        Assert.Contains("grant_type=password", call.Body);
        Assert.StartsWith("application/x-www-form-urlencoded", call.Request.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Login_InvalidResponse_ReturnsEmptyToken()
    {
        var handler = new FakeHandler((_, _) => new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("not json")
        });
        var client = new Client<Guid>(Data, new HttpClient(handler));

        var token = await client.Login("u", "p");

        Assert.Null(token.AccessToken);
    }

    [Fact]
    public async Task Logout_ReturnsTrueOnNoContent_AndSendsBearer()
    {
        var handler = new FakeHandler((_, _) => new HttpResponseMessage(HttpStatusCode.NoContent));
        var client = new Client<Guid>(Data, new HttpClient(handler));

        bool result = await client.Logout(new Logout { AccessToken = "at", RefreshToken = "rt" });

        Assert.True(result);
        var call = handler.Calls.Single();
        Assert.Equal("Bearer", call.Request.Headers.Authorization!.Scheme);
        Assert.Equal("at", call.Request.Headers.Authorization.Parameter);
        Assert.Contains("refresh_token=rt", call.Body);
        Assert.StartsWith(Data.BaseUrl, call.Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteUser_BuildsUrlWithGuid()
    {
        var handler = new FakeHandler((_, _) => new HttpResponseMessage(HttpStatusCode.NoContent));
        var client = new Client<Guid>(Data, new HttpClient(handler));
        var id = Guid.NewGuid();

        bool result = await client.DeleteUser(new DeleteUser<Guid> { AccessToken = "at", UserGuid = id });

        Assert.True(result);
        Assert.Equal($"{Data.BaseUrl}{KeycloakConstants.UserEndpoint}/{id}", handler.Calls.Single().Request.RequestUri!.ToString());
        Assert.Equal(HttpMethod.Delete, handler.Calls.Single().Request.Method);
    }

    [Fact]
    public async Task Registration_CreatesUserSetsPasswordAndLogsIn()
    {
        var userId = Guid.NewGuid();
        var handler = new FakeHandler((req, _) =>
        {
            string url = req.RequestUri!.ToString();
            if (url.EndsWith(KeycloakConstants.LoginEndpoint))
            {
                return Json("{\"access_token\":\"tok\"}");
            }
            if (req.Method == HttpMethod.Post && url.EndsWith(KeycloakConstants.UserEndpoint))
            {
                var r = new HttpResponseMessage(HttpStatusCode.Created);
                r.Headers.Location = new Uri($"http://localhost:8080/{KeycloakConstants.UserEndpoint}/{userId}");
                return r;
            }
            if (req.Method == HttpMethod.Put && url.EndsWith("/reset-password"))
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        var client = new Client<Guid>(Data, new HttpClient(handler));

        var token = await client.Registration(new Registration
        {
            Email = "a@b.c",
            Username = "newuser",
            Password = "pw",
            Enabled = true
        });

        Assert.Equal("tok", token.AccessToken);
        var create = handler.Calls.First(c => c.Request.Method == HttpMethod.Post && c.Request.RequestUri!.ToString().EndsWith(KeycloakConstants.UserEndpoint));
        Assert.Contains("\"enabled\":true", create.Body);
        Assert.Contains($"/{userId}/reset-password", handler.Calls.Single(c => c.Request.Method == HttpMethod.Put).Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task CreateClient_ReturnsTrueOnCreated()
    {
        var handler = new FakeHandler((_, _) => new HttpResponseMessage(HttpStatusCode.Created));
        var client = new Client<Guid>(Data, new HttpClient(handler));

        bool result = await client.CreateClient(new KeycloakClient { ClientId = "new", Enabled = true }, "at");

        Assert.True(result);
        Assert.Contains("\"clientId\":\"new\"", handler.Calls.Single().Body);
    }

    [Fact]
    public async Task GetAllClients_ParsesList_AndReturnsEmptyOnFailure()
    {
        var ok = new FakeHandler((_, _) => Json("[{\"clientId\":\"a\"},{\"clientId\":\"b\"}]"));
        var list = await new Client<Guid>(Data, new HttpClient(ok)).GetAllClients("at");
        Assert.Equal(2, list.Count);

        var fail = new FakeHandler((_, _) => new HttpResponseMessage(HttpStatusCode.Forbidden));
        var empty = await new Client<Guid>(Data, new HttpClient(fail)).GetAllClients("at");
        Assert.Empty(empty);
    }

    [Fact]
    public async Task UpdateAndDeleteClient_UseClientGuidInUrl()
    {
        var handler = new FakeHandler((_, _) => new HttpResponseMessage(HttpStatusCode.NoContent));
        var client = new Client<Guid>(Data, new HttpClient(handler));

        Assert.True(await client.UpdateClient(new KeycloakClient { ClientId = "x" }, "at", "guid1"));
        Assert.True(await client.DeleteClient("guid2", "at"));

        Assert.EndsWith(KeycloakConstants.ClientEndpoint + "/guid1", handler.Calls[0].Request.RequestUri!.ToString());
        Assert.EndsWith(KeycloakConstants.ClientEndpoint + "/guid2", handler.Calls[1].Request.RequestUri!.ToString());
    }
}
