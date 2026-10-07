# KeycloakNet10.Demo

A Windows-only demo (`net10.0-windows`, WinForms plus console) for the [KeycloakNet10](../KeycloakNet10/README.md) library.

## What it does
1. Shows a login dialog (`LoginForm`) that asks for:
   - Base URL (default `http://localhost:8080/`)
   - Client ID and Client Secret
   - Admin username and password
   - User username and password
2. Creates a `Client<Guid>` from the values.
3. Logs in with the user credentials, and shows a message box saying "Login succeeded" or "Login failed".
4. On success, lists the Keycloak clients and prints the count to the console.
5. Logs out and prints the result to the console.

All fields are required. Cancelling the dialog exits the app.

## Prerequisites
- Windows with the .NET 10 SDK
- A running Keycloak server (`master` realm)
- A client with a secret and *Direct Access Grants* enabled
- A user to log in as

## Run
```
dotnet run --project KeycloakNet10.Demo
```
Or set `KeycloakNet10.Demo` as the startup project in Visual Studio and press F5.

## Structure
- `Program.cs`: top-level flow. The dialog runs on an STA thread.
- `LoginForm.cs`: the input dialog.

## Notes
- The admin credentials are only needed for `Registration`, but the form requires them.
- The demo doesn't call `Registration`, `DeleteUser`, `CreateClient`, `UpdateClient` or `DeleteClient`.
