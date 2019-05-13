# KeycloakStandard
Keycloak wrapper in .NET Standard

### Getting started
KeycloakStandard is a library for working with Keycloak from .NET Standard 2.0, which means
you can call this library from .NET Framework (>= 4.6.1), .NET Core (>= 2.0) and others.

[Check](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support) if your project is compliant with .NET Standard 2.0
## Getting started

1) To start with this library, you need to install this from Nuget via Package Manager:

**Install-Package KeycloakStandard**

2) To begin with the coding part, you need to first make an instance of ClientData object that resides under
    "KeycloakStandard.Models" namespace and fill every property under that object:

```csharp
ClientData clientData = new ClientData()
{
    ...
};
```

3) Next, you need to make an instance of Client object that resides under
    "KeycloakStandard" namespace:

```csharp
Client<TUserIdType> client = new Client<TUserIdType>(clientData);
```

Notice TUserIdType in that line. I have left the room for that. By default user Id in Keycloak database is **varying(36)**
(PostgreSQL) which means that you will place **string** instead of TUserIdType. The reason behind this generic is that maybe there are
modified versions of Keycloak where someone placed **int** as a data type for user Id.

## Login

To authenticate (login) user in the Keycloak all the information you need is username and password of that user:

```csharp
var tokenResult = await client.KeycloakLogin("TestUsername", "TestPassword");
```

## User registration

To register new user in the Keycloak database, you need to make an instance of Registration object that resides under 
"KeycloakStandard.Models" namespace and fill the data:

```csharp
Registration registration = new Registration
{
    ...
};
```
Then, you pass that Registration object to KeycloakRegistration method:

```csharp
var tokenResult = await client.KeycloakRegistration(registration);
```

## Logout user

First, you need to make an instance of Logout object and fill the data. Logout resides under "KeycloakStandard.Models" namespace:

```csharp
Logout logout = new Logout()
{
    ...
};
```

To logout currently logged user in, you will work with **KeycloakLogout** method and pass created Logout object as
a parameter to that method:

```csharp
bool isLoggedOut = await client.KeycloakLogout(logout);
```

## Delete user

First, you need to make an instance of DeleteUser object and fill the data. DeleteUser resides under "KeycloakStandard.Models" namespace:

```csharp
DeleteUser<TUserIdType> deleteUser = new DeleteUser<TUserIdType>()
{
    ...
};
```

Explanation for TUserIdType in this example is the same as in the example above.

To delete existing user, you will work with **KeycloakDeleteUser** method and pass created DeleteUser object as
a parameter to that method:

```csharp
bool isUserDeleted = await client.KeycloakDeleteUser(deleteUser);
```


## Donations 

If you thin this project is good and want to show your support for further development of this repository, you can donate any amount of BTC. If you don't feel with donating, you can be a contributor and help me that way.

<div style="text-align:center">
    <a href="https://blockchain.info/address/322SRqTS3EeKGaVFuo6xsw8e5Xji4QcJR6">

    Click to see my BTC address
    
</div>
