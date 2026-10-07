using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeycloakNet10.Models;

public class ClientData
{
    public string BaseUrl { get; set; } = string.Empty;
    public string AdminUsername { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class DeleteUser<TId> where TId : struct
{
    public string AccessToken { get; set; } = string.Empty;
    public TId UserGuid { get; set; }
}

public class Logout
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class Registration
{
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public bool Enabled { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Temporary { get; set; }
}

public class KeycloakClient
{
    [JsonPropertyName("attributes")]
    public JsonElement? Attributes { get; set; }

    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    [JsonPropertyName("redirectUris")]
    public JsonElement? RedirectUris { get; set; }

    [JsonPropertyName("rootUrl")]
    public string? RootUrl { get; set; }
}

/// <summary>
/// Token information from Keycloak.
/// </summary>
public class KeycloakToken
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("not-before-policy")]
    public int NotBeforePolicy { get; set; }

    [JsonPropertyName("session_state")]
    public string? SessionState { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}
