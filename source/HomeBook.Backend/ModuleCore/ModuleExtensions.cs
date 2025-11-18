using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Modules.Abstractions;
using HomeBook.Backend.Options;

namespace HomeBook.Backend.ModuleCore;

public static class ModuleExtensions
{
    private static ModuleBuilder? _moduleBuilder = null;

    /// <summary>
    /// use in Blazor Server
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="homeBookOptions"></param>
    /// <param name="builderAction"></param>
    public static void AddModules(this WebApplicationBuilder builder,
        HomeBookOptions homeBookOptions,
        Action<ModuleBuilder> builderAction)
    {
        builder.Services.AddModules(
            homeBookOptions,
            builder.Configuration,
            builderAction);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sc"></param>
    /// <param name="hb"></param>
    /// <param name="c"></param>
    /// <param name="builderAction"></param>
    public static void AddModules(this IServiceCollection sc,
        HomeBookOptions hb,
        IConfiguration c,
        Action<ModuleBuilder> builderAction)
    {
        _moduleBuilder = new ModuleBuilder(hb, sc, c);
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
    public static async Task RunModulesPostBuild(this WebApplication host)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        await host.RunModulesPostBuild(host.Services,
            host.Configuration);

        // call startup service if needed
    }

    /// <summary>
    /// general post build logic
    /// </summary>
    /// <param name="host"></param>
    /// <param name="sp"></param>
    /// <param name="c"></param>
    public static async Task RunModulesPostBuild(this WebApplication host,
        IServiceProvider sp,
        IConfiguration c)
    {
        IEnumerable<IModule> modules = sp.GetServices<IModule>();

        // initialize all modules
        foreach (IModule module in modules)
        {
            if (_moduleBuilder is null)
                return;

            // register endpoints
            try
            {
                await host.RegisterEndpointsForModuleAsync(module);
            }
            catch (NotImplementedException)
            {
                // do nothing
            }

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

    public static async Task RegisterEndpointsForModuleAsync(this WebApplication host,
        IModule module)
    {
        if (module is not IBackendModuleEndpointRegistrar registrar)
            return;
        IBackendModuleEndpointRegistrar endpointRegistrar = registrar;

        IConfiguration configuration = host.Configuration;

        // register endpoint group for module
        RouteGroupBuilder moduleEndpointGroup = host.MapGroup($"/modules/{module.Key}")
            .WithDescription(module.Description)
            .WithTags([
                module.Name
            ]);

        IEndpointBuilder builder = new EndpointBuilder(moduleEndpointGroup);
        endpointRegistrar.RegisterEndpoints(builder, configuration);

        IEndpointDataAccessor endpointDataAccessor = (IEndpointDataAccessor)builder;
    }
}
