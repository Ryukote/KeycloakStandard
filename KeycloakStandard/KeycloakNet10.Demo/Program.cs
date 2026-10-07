using KeycloakNet10.Demo;
using KeycloakNet10.Models;
using KeycloakNet10;  

LoginForm? form = null;
var accepted = false;
var uiThread = new Thread(() =>
{
    ApplicationConfiguration.Initialize();
    form = new LoginForm();
    accepted = form.ShowDialog() == DialogResult.OK;
});
uiThread.SetApartmentState(ApartmentState.STA);
uiThread.Start();
uiThread.Join();

if (!accepted || form is null)
{
    Console.WriteLine("Cancelled");
    return;
}

var client = new Client<Guid>(new ClientData
{
    BaseUrl = form.BaseUrl,
    ClientId = form.ClientId,
    ClientSecret = form.ClientSecret,
    AdminUsername = form.AdminUsername,
    AdminPassword = form.AdminPassword
});
try
{
    var token = await client.Login(form.Username, form.Password);

if (string.IsNullOrEmpty(token.AccessToken))
{
    Console.WriteLine("Login failed");
    MessageBox.Show("Login failed", "Keycloak Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}

Console.WriteLine("Login succeeded");
MessageBox.Show("Login succeeded", "Keycloak Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
var clients = await client.GetAllClients(token.AccessToken);
Console.WriteLine($"Clients found: {clients.Count}");
var loggedOut = await client.Logout(new Logout { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken ?? string.Empty });
Console.WriteLine(loggedOut ? "Logout succeeded" : "Logout failed");
    MessageBox.Show(loggedOut ? "Logout succeeded" : "Logout failed", "Keycloak Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Cannot reach Keycloak at {form.BaseUrl}: {ex.Message}");
    MessageBox.Show("Login failed", "Keycloak Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}
