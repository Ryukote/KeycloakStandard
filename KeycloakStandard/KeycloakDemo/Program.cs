using KeycloakStandard;
using KeycloakStandard.Models;

var client = new Client<int>(new ClientData
{
    BaseUrl = "http://localhost:8080/",
    ClientId = "admin",
    ClientSecret = "UjLNDY8iampmCIiM2DhjJDyigByMdHLthNEYIVasMmWDq9AxP4Kw2QvbrEdN7yIMvsuCKGNszddz9AYM8ms2bO",
    AdminUsername = "admin",
    AdminPassword = "admin"
});

var token = await client.Login("admin", "admin");
Console.WriteLine(string.IsNullOrEmpty(token?.AccessToken)
    ? "Login failed"
    : "Login succeeded");
