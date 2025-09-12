using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Endpoints;
using HomeBook.Backend.EnvironmentHandler;
using HomeBook.Backend.Extensions;
using HomeBook.Backend.Core.Extensions;
using HomeBook.Backend.Core.Account.Extensions;
using Scalar.AspNetCore;
using Serilog;

#if DEBUG
string developmentEnvFile = $"env{Path.DirectorySeparatorChar}Development.env";
EnvironmentLoader.LoadEnvFile(developmentEnvFile);
#endif

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.jwt.json", optional: true, reloadOnChange: true)
    .AddJsonFile(PathHandler.RuntimeConfigurationFilePath, optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(prefix: "HB_");

// Serilog einrichten
builder.Host.UseSerilog((ctx, services, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddOpenApi();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

InstanceStatus instanceStatus = builder.Configuration.GetCurrentInstanceStatus();

switch (instanceStatus)
{
    // map endpoints that are only available in setup mode
    case InstanceStatus.SETUP:
        builder.Services.AddBackendSetup(builder.Configuration, instanceStatus);
        break;
    // map endpoints that are only available in running mode
    case InstanceStatus.RUNNING:
        builder.Services.AddBackendServices(builder.Configuration, instanceStatus)
            .AddBackendCore(builder.Configuration, instanceStatus)
            .AddBackendDatabaseProvider(builder.Configuration, instanceStatus)
            .AddAccountServices(builder.Configuration, instanceStatus);
        break;
}

builder.Services.AddJwtAuthentication(builder.Configuration, instanceStatus);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}

var app = builder.Build();

// Log application startup - guaranteed to be written to Serilog text file
Log.Information("HomeBook Backend application starting up - Version: {Version}",
    app.Configuration["Version"] ?? "Unknown");

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseDefaultFiles();
// app.UseStaticFiles();
// app.MapFallbackToFile("index.html"); // <- important for Blazor Routing

#region map endpoints

// map endpoints that are always available
app.MapVersionEndpoints()
    .MapSystemEndpoints();

switch (instanceStatus)
{
    // map endpoints that are only available in setup mode
    case InstanceStatus.SETUP:
        app.MapSetupEndpoints();
        break;
    // map endpoints that are only available in running mode
    case InstanceStatus.RUNNING:
        app.MapSetupEndpoints()
            .MapUpdateEndpoints()
            .MapAccountEndpoints();
        break;
}

#endregion

app.Run();
