using HomeBook.Frontend.Modules.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Frontend.Module.Kitchen;

/// <summary>
/// the recipes and pantry module
/// </summary>
public class Module
    : IModule,
        IModuleWidgetRegistration,
        IModuleDependencyRegistration,
        IModuleStartMenuRegistration
{
    /// <inheritdoc />
    public string Name => "KitchenManager";

    /// <inheritdoc />
    public string Description => "Module for managing recipes, meal planning and pantry.";

    /// <inheritdoc />
    public string Author { get; } = "HomeBook";

    /// <inheritdoc />
    public Version Version => new("1.0.0");

    /// <inheritdoc />
    public string Icon { get; } = HomeBook.Frontend.Core.Icons.HomeBookIcons.Icons8.GlassMorphism.Tableware;

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public static void RegisterWidgets(IWidgetBuilder builder,
        IConfiguration configuration)
    {
        // builder.AddWidget<Widgets.CurrentBudgetWidget>();
    }

    /// <inheritdoc />
    public static void RegisterServices(IServiceCollection services,
        IConfiguration configuration)
    {
    }

    /// <inheritdoc />
    public static void RegisterStartMenuItems(IStartMenuBuilder builder,
        IConfiguration configuration)
    {
        builder.AddStartMenu("Module_KitchenManager_StartMenuItem_Recipes_Title",
            "Module_KitchenManager_StartMenuItem_Recipes_Caption",
            "/Kitchen/Recipes",
            HomeBook.Frontend.Core.Icons.HomeBookIcons.Icons8.Windows11.Filled.CookBook,
            "#FF9800");

        builder.AddStartMenu("Module_KitchenManager_StartMenuItem_Pantry_Title",
            "Module_KitchenManager_StartMenuItem_Pantry_Caption",
            "/Kitchen/Pantry",
            HomeBook.Frontend.Core.Icons.HomeBookIcons.Icons8.Windows11.Filled.GroceryShelf,
            "#009688");
    }
}
