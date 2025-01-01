# StartSCH

## Development
### Configuration
- [Docs: Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration)
- [Docs: Options pattern in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)

#### Push notifications (optional)
- [MDN: Web Push API](https://developer.mozilla.org/en-US/docs/Web/API/Push_API)
- [web.dev: Push notifications overview](https://web.dev/articles/push-notifications-overview)

Some push services might require that the server identifies itself with a 
[VAPID](https://datatracker.ietf.org/doc/html/rfc8292) key pair.
[It is required by Apple](https://developer.apple.com/documentation/usernotifications/sending-web-push-notifications-in-web-apps-and-browsers#Prepare-your-server-to-send-push-notifications).
Keys can be generated [here](https://web-push-codelab.glitch.me/).

```sh
dotnet user-secrets set Push__PublicKey "..."
dotnet user-secrets set Push__PrivateKey "..."
dotnet user-secrets set Push__Subject "mailto:..."
```

### Database
#### Migrations
- [Docs: Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations)
- [Docs: Migrations with Multiple Providers](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers)

After modifying the `Db`, you have to create new migrations:
```sh
# Go to the server project directory (e.g. ~/src/StartSch/StartSch)
cd StartSch

# Make sure you can run `dotnet ef`.
# One of these commands, ideally the first one, should install it.
dotnet tool restore
dotnet tool install dotnet-ef
dotnet tool install dotnet-ef --global

# Describe the migration
export MIGRATION_MESSAGE=AddSomethingToSomeOtherThing

# Add migration
dotnet ef migrations add --context SqliteDb $MIGRATION_MESSAGE
dotnet ef migrations add --context PostgresDb $MIGRATION_MESSAGE

# Remove latest migration
dotnet ef migrations remove --context SqliteDb
dotnet ef migrations remove --context PostgresDb

# Reset migrations
rm -r Data/Migrations
dotnet ef migrations add --context SqliteDb --output-dir Data/Migrations/Sqlite $MIGRATION_MESSAGE
dotnet ef migrations add --context PostgresDb --output-dir Data/Migrations/Postgres $MIGRATION_MESSAGE
```

Migrations are applied automatically on server startup.

#### Injecting a `Db` instance
- [Docs: DbContext Lifetime, Configuration, and Initialization](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/)
- [Docs: ASP.NET Core Blazor with Entity Framework Core](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-ef-core)

Depending on where you want to access the database, you have to decide between injecting `Db` or `IDbContextFactory<Db>`.

For example, static forms or API controllers that run in a scope should use `Db`, while methods in an interactive Blazor component should request a new `Db` instance every time they run.

[![](https://i.kym-cdn.com/entries/icons/original/000/044/268/shoescover.jpg)](https://knowyourmeme.com/memes/if-your-boss-lawyers-pants-looks-like-this)
