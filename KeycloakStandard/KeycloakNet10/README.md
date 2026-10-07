# KeycloakNet10

A lightweight .NET 10 client library for the Keycloak REST API. It handles authentication and basic user and client administration. It has no NuGet dependencies.

## Features
- Login (OAuth2 password grant)
- Logout
- Register a user (create, set password, login)
- Delete a user
- Create, list, update and delete Keycloak clients

## Usage

```csharp
using KeycloakNet10;
using KeycloakNet10.Models;

var client = new Client<Guid>(new ClientData
{
	BaseUrl = "http://localhost:8080/",   // must end with '/'
	ClientId = "my-client",
	ClientSecret = "secret",
	AdminUsername = "admin",
	AdminPassword = "admin"
});

var token = await client.Login("user", "password");
if (string.IsNullOrEmpty(token.AccessToken))
{
	// login failed
}

var clients = await client.GetAllClients(token.AccessToken!);
await client.Logout(new Logout { AccessToken = token.AccessToken!, RefreshToken = token.RefreshToken ?? "" });
```

Pass a shared `HttpClient` (for example from `IHttpClientFactory`) as the second constructor argument. If you omit it, the client creates its own.

## API

| Method | Description | Returns |
|---|---|---|
| `Login(username, password)` | Gets a token. | `KeycloakToken`, empty if the login failed |
| `Registration(registration)` | Creates a user using the admin account, sets the password, then logs in as that user. | `KeycloakToken`, empty on failure |
| `Logout(logout)` | Invalidates the refresh token. | `bool` |
| `DeleteUser(deleteUser)` | Deletes a user. | `bool` |
| `CreateClient(client, accessToken)` | Creates a Keycloak client. | `bool` |
| `GetAllClients(accessToken)` | Lists Keycloak clients. | `ICollection<KeycloakClient>` |
| `UpdateClient(client, accessToken, clientGuid)` | Updates a Keycloak client. | `bool` |
| `DeleteClient(clientGuid, accessToken)` | Deletes a Keycloak client. | `bool` |

Authentication failures don't throw. Methods return an empty token (`AccessToken == null`), `false` or an empty list.

## Structure
- `Client.cs`: the `Client<TUserIdType>` class.
- `KeycloakConstants.cs`: endpoint paths.
- `Models/Models.cs`: `ClientData`, `KeycloakToken`, `Registration`, `Logout`, `DeleteUser<T>` and `KeycloakClient`.

## Limitations
- Endpoints are hardcoded to the `master` realm.
- `BaseUrl` must end with `/`.
- The password grant is deprecated in newer OAuth guidance.
- The paths have no `/auth` prefix, so they match Keycloak 17 and later. For older versions, include `auth/` in `BaseUrl`.
