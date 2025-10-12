using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Modules.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HomeBook.Frontend.ModuleCore;

public static class ModuleExtensions
{
    private static ModuleBuilder? _moduleBuilder = null;

    /// <summary>
    /// use in Blazor Server
    /// </summary>
    /// <param name="host"></param>
    /// <param name="configureOptions"></param>
    public static void AddModules(this WebAssemblyHostBuilder builder,
        Action<ModuleBuilder> builderAction)
    {
        builder.Services.AddModules(builder.Configuration,
            builderAction);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sc"></param>
    /// <param name="c"></param>
    /// <param name="builderAction"></param>
    public static void AddModules(this IServiceCollection sc,
        IConfiguration c,
        Action<ModuleBuilder> builderAction)
    {
        _moduleBuilder = new ModuleBuilder(sc, c);
        builderAction(_moduleBuilder);

        // _moduleBuilder
        //     .AddSystemModule<HomeModule>()
        //     .AddSystemModule<HelpModule>();

        // _modules = sp.GetServices<IModule>();
    }

    /// <summary>
    /// use in Blazor Server
    /// </summary>
    /// <param name="host"></param>
    public static async Task RunModulesPostBuild(this WebAssemblyHost host)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        await host.Services.RunSupportModulesPostBuild(host.Configuration);

        IStartupService startupService = host.Services.GetRequiredService<IStartupService>();
        await startupService.StartAsync(cancellationToken);
    }

    /// <summary>
    /// general post build logic
    /// </summary>
    /// <param name="sp"></param>
    public static async Task RunSupportModulesPostBuild(this IServiceProvider sp,
        IConfiguration configuration)
    {
        IEnumerable<IModule> modules = sp.GetServices<IModule>();
        IWidgetFactory widgetFactory = sp.GetRequiredService<IWidgetFactory>();

        // initialize all modules
        foreach (IModule module in modules)
        {
            if (_moduleBuilder is null)
                return;

            // register widgets
            string moduleId = module.GetType().FullName
                              ?? throw new InvalidOperationException("Module type must have a full name.");
            // IReadOnlyList<Type> widgets = widgetFactory.GetWidgetTypesForModule(moduleId);

            IWidgetBuilder widgetBuilder = _moduleBuilder.GetWidgetBuilder(moduleId);
            widgetFactory.AddWidgetBuilder(moduleId, widgetBuilder);

            // call the initialization logic
            try
            {
                await module.InitializeAsync();
            }
            catch (NotImplementedException)
            {
                // do nothing
            }
        }
    }
}
