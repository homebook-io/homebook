using System.Reflection;
using HomeBook.Frontend.Abstractions.Contracts;
using HomeBook.Frontend.Modules.Abstractions;

namespace HomeBook.Frontend.ModuleCore;

public class ModuleBuilder(
    IServiceCollection serviceCollection,
    IConfiguration configuration)
{
    private Dictionary<string, IWidgetBuilder> _registeredWidgets = new();

    /// <summary>
    /// adds a module to the service collection if the module is enabled.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ModuleBuilder AddModule<T>() where T : class, IModule
    {
        // IFeatureManager featureManager = serviceCollection
        //     .BuildServiceProvider()
        //     .GetRequiredService<IFeatureManager>();

        string moduleId = typeof(T).FullName
                          ?? throw new InvalidOperationException("Module type must have a full name.");

        // bool isEnabled = featureManager.IsEnabledAsync(moduleId).GetAwaiter().GetResult();
        // if (!isEnabled)
        // {
        //     return this;
        // }

        // register the module
        RegisterModule<T>(moduleId);

        // register the modules widgets
        IWidgetBuilder widgetBuilder = new WidgetBuilder<T>();
        RegisterModuleWidgets<T>(widgetBuilder, moduleId);

        _registeredWidgets.Add(moduleId, widgetBuilder);

        return this;
    }

    private void RegisterModuleWidgets<T>(IWidgetBuilder widgetBuilder,
        string moduleId) where T : class, IModule
    {
        // implements the Module the IModuleWidgetRegistration interface?
        if (!typeof(IModuleWidgetRegistration).IsAssignableFrom(typeof(T)))
            return;

        // call the RegisterWidgets method in the module
        MethodInfo? method = typeof(T).GetMethod(
            "RegisterWidgets",
            BindingFlags.Public | BindingFlags.Static
        );
        method?.Invoke(null, [widgetBuilder, configuration]);
    }

    private void RegisterModule<T>(string moduleId) where T : class, IModule
    {
        // register the IModule itself
        serviceCollection.AddSingleton<IModule, T>();
        serviceCollection.AddKeyedSingleton<IModule, T>(moduleId);

        // implements the Module the IModuleDependencyRegistration interface?
        if (!typeof(IModuleDependencyRegistration).IsAssignableFrom(typeof(T)))
            return;

        // call the RegisterServices method in the module
        MethodInfo? method = typeof(T).GetMethod(
            "RegisterServices",
            BindingFlags.Public | BindingFlags.Static
        );
        method?.Invoke(null, [serviceCollection, configuration]);
    }

    public IWidgetBuilder GetWidgetBuilder(string moduleId) => _registeredWidgets[moduleId];
}
