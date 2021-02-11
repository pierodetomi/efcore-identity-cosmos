![Logo](https://raw.githubusercontent.com/pierodetomi/efcore-identity-cosmos/main/PieroDeTomi.EntityFrameworkCore.Identity.Cosmos/_res/icons/nuget-icon.png)

# EF Core Cosmos DB Identity Provider
This is a **Cosmos DB** implementation of an Identity provider for .NET Core 5, using the official [EF Core Azure Cosmos DB Provider](https://docs.microsoft.com/en-us/ef/core/providers/cosmos/?tabs=dotnet-core-cli).

You can use this package to easily bootstrap an ASP.NET **Identity Server** backed by a CosmosDb database and EF Core, in place of SQL Server.

# Installation (NuGet)

```shell
PM> Install-Package PieroDeTomi.EntityFrameworkCore.Identity.Cosmos
```

# Integration Steps

## Project Requirements
The following steps assume that you have an ASP.NET Core 5 Web Application (or SPA Web Application) project with _authentication_ + _individual accounts_ feature turned on.

## Cosmos DB Requirements
### Database
Just as with EF Core on SQL Server, you have to manually create a database in your Cosmos DB instance to be able to operate.
### Containers
Since migrations are **NOT** supported when using EF Core on Cosmos DB, you'll have to _manually_ create the following containers:

| Container Name | Partition Key |
| --- | --- |
| Identity | /Id |
| Identity_DeviceFlowCodes | /SessionId |
| Identity_Logins | /ProviderKey |
| Identity_PersistedGrant | /Key |
| Identity_Tokens | /UserId |
| Identity_UserRoles | /UserId |

## DbContext
You have to create a DbContext (e.g.: the DbContext you'll use in your application) that implements the ```CosmosIdentityDbContext``` type.

If you just want to start with authentication and add the other entities later, you just need to create an empty DbContext class that satisfies the above requirement:

```csharp
public class MyDbContext : CosmosIdentityDbContext<IdentityUser>
{
  public MyDbContext(DbContextOptions dbContextOptions, IOptions<OperationalStoreOptions> options)
    : base(dbContextOptions, options) { }
}
```

## Startup.cs File Configuration
Remove the line where the current identity provider is added/configured.

If you just created a new project, this line should be something like:

```csharp
services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
  .AddEntityFrameworkStores<ApplicationDbContext>();
```

Now add the Cosmos DB provider:

```csharp
services.AddCosmosIdentity<MyDbContext, IdentityUser, IdentityRole>(
  // Auth provider standard configuration (e.g.: account confirmation, password requirements, etc.)
  options => ...,
  options => options.UseCosmos(
      "your_cosmos_db_URL",
      "your_cosmos_db_key",
      databaseName: "your_db"
  ),

  // If true, AddDefaultTokenProviders() method will be called on the IdentityBuilder instance
  addDefaultTokenProviders: false   
);
```

If your project is using a SPA Web Application template (e.g.: the Angular web app), update the **Identity Server** configuration, in order to use your new DbContext implementation:

```csharp
// Note that we're using MyDbContext as the second type parameter here...
services.AddIdentityServer().AddApiAuthorization<IdentityUser, MyDbContext>();
```

# Available Services
This library registers in the service collection a basic Cosmos DB repository implementation, that you can resolve in your constructors requiring the `IRepository` interface.

An example:

```csharp
public class MyClass {
  private readonly IRepository _repo;

  public MyClass(IRepository repo) {
    _repo = repo;
  }

  // ... Use the _repo instance methods to query the database
}
```

## Available IRepository methods
Just for your information, here is a summary of the available methods in the IRepository interface:

- ```Table<TEntity>()```
- ```GetById<TEntity>(string id)```
- ```TryFindOne<TEntity>(Expression<Func<TEntity, bool>> predicate)```
- ```Find<TEntity>(Expression<Func<TEntity, bool>> predicate)```
- ```Add<TEntity>(TEntity entity)```
- ```Update<TEntity>(TEntity entity)```
- ```DeleteById<TEntity>(string id)```
- ```Delete<TEntity>(TEntity entity)```
- ```Delete<TEntity>(Expression<Func<TEntity, bool>> predicate)```
- ```SaveChangesAsync()```