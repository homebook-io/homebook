using HomeBook.Frontend.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Frontend.Module.Finances;

/// <summary>
/// the finances module
/// </summary>
public class Module : IModule, IModuleWidgetRegistration, IModuleDependencyRegistration
{
    /// <inheritdoc />
    public string Name => "Finances";

    /// <inheritdoc />
    public string Description => "Module for managing finances, including expenses and budgets.";

    /// <inheritdoc />
    public string Author { get; } = "HomeBook";

    /// <inheritdoc />
    public Version Version => new("1.0.0");

    /// <inheritdoc />
    public string Icon { get; } = HomeBook.Frontend.Core.Icons.HomeBookIcons.Icons8.LiquidGlassColor.Investment;

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public static void RegisterWidgets(IWidgetBuilder builder,
        IConfiguration configuration)
    {
        builder.AddWidget<Widgets.CurrentBudgetWidget>();
    }

    public static void RegisterServices(IServiceCollection services,
        IConfiguration configuration)
    {
    }
}
