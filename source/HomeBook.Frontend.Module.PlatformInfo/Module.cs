using HomeBook.Frontend.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Frontend.Module.PlatformInfo;

/// <summary>
/// the platform info module
/// </summary>
public class Module : IModule, IModuleWidgetRegistration, IModuleDependencyRegistration
{
    /// <inheritdoc />
    public string Name => "Platform Info";

    /// <inheritdoc />
    public string Description => "Provides information about the platform and environment.";

    /// <inheritdoc />
    public string Author { get; } = "HomeBook";

    /// <inheritdoc />
    public Version Version => new("1.0.0");

    /// <inheritdoc />
    public string Icon { get; } = HomeBook.Frontend.Core.Icons.HomeBookIcons.Icons8.LiquidGlassColor.Help;

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public static void RegisterWidgets(IWidgetBuilder builder, IConfiguration configuration)
    {

    }

    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {

    }
}
