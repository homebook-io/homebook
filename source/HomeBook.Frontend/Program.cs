using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using HomeBook.Frontend;
using HomeBook.Frontend.Extensions;
using HomeBook.Frontend.Services.Extensions;
using HomeBook.Frontend.Provider;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    {
        "Frontend:Host", builder.HostEnvironment.BaseAddress
    }
});

builder.Services.AddScoped(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
    string? webAddress = configuration["Frontend:Host"];

    if (string.IsNullOrEmpty(webAddress))
        throw new ArgumentNullException($"Frontend:Host is not configured");

    return new HttpClient
    {
        BaseAddress = new Uri(webAddress)
    };
});

// Add Authorization services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddMudServices(x =>
{
    x.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
});
builder.Services.AddFrontendUiServices(builder.Configuration)
    .AddFrontendServices(builder.Configuration)
    .AddBackendClient(builder.Configuration);

await builder.Build().RunAsync();
