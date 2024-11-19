using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using StartSch;
using StartSch.Auth;
using StartSch.Components;
using StartSch.Data;
using StartSch.Modules.Cmsch;
using StartSch.Modules.GeneralEvent;
using StartSch.Modules.SchPincer;
using StartSch.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddDataProtection().PersistKeysToDbContext<Db>();

// Authentication and authorization
builder.Services.AddAuthSch();

// Blazor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

// Database
//    Register SqliteDb
builder.Services.AddPooledDbContextFactory<SqliteDb>(db =>
{
    db.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=StartSch.db");
    if (builder.Environment.IsDevelopment()) db.EnableSensitiveDataLogging();
});

//    Register PostgresDb if there is a connection string
string? postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");
if (postgresConnectionString != null)
{
    builder.Services.AddPooledDbContextFactory<PostgresDb>(db =>
    {
        db.UseNpgsql(postgresConnectionString);
        if (builder.Environment.IsDevelopment()) db.EnableSensitiveDataLogging();
    });
}

//    Register Db and IDbContextFactory<Db> using one of them
if (postgresConnectionString != null)
{
    builder.Services.AddSingleton<IDbContextFactory<Db>>(sp =>
        new DbContextFactoryTranslator<PostgresDb>(sp.GetRequiredService<IDbContextFactory<PostgresDb>>()));
    builder.Services.AddScoped<Db>(sp => sp.GetRequiredService<IDbContextFactory<PostgresDb>>().CreateDbContext());
}
else
{
    builder.Services.AddSingleton<IDbContextFactory<Db>>(sp =>
        new DbContextFactoryTranslator<SqliteDb>(sp.GetRequiredService<IDbContextFactory<SqliteDb>>()));
    builder.Services.AddScoped<Db>(sp => sp.GetRequiredService<IDbContextFactory<SqliteDb>>().CreateDbContext());
}

// Email service
string? kirMailApiKey = builder.Configuration["KirMailApiKey"];
if (kirMailApiKey != null)
{
    builder.Services.AddHttpClient(nameof(KirMailService),
        client =>
        {
            client.DefaultRequestHeaders.Authorization = new("Api-Key", kirMailApiKey);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("StartSCH/1 (https://start.alb1.hu)");
        });
    builder.Services.AddSingleton<IEmailService, KirMailService>();
}
else
    builder.Services.AddSingleton<IEmailService, DummyEmailService>();

// Push notifications
// https://tpeczek.github.io/Lib.Net.Http.WebPush/articles/aspnetcore-integration.html
builder.Services.AddMemoryCache();
builder.Services.AddMemoryVapidTokenCache();
builder.Services.AddPushServiceClient(builder.Configuration.GetSection("Push").Bind);
builder.Services.AddScoped<PushService>();

// Modules
builder.Services.AddModule<CmschModule>();
builder.Services.AddModule<GeneralEventModule>();
builder.Services.AddModule<SchPincerModule>();
builder.Services.AddHostedService<CronService>();

var app = builder.Build();

{
    await using var serviceScope = app.Services.CreateAsyncScope();
    await serviceScope.ServiceProvider.GetRequiredService<Db>().Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(StartSch.Wasm._Imports).Assembly);

app.Run();