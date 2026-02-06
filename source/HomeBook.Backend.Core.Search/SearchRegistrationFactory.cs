using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Modules.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Core.Search;

public class SearchRegistrationFactory()
    : ISearchRegistrationFactory,
        ISearchRegistrationInitiator
{
    private IServiceProvider _serviceProvider = null!;
    private readonly List<string> _registeredModules = [];

    public void AddModule(string moduleId) => _registeredModules.Add(moduleId);

    public void AddServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public ISearchProvider CreateSearchProvider()
    {
        List<IBackendModuleSearchRegistrar> modules = [];
        foreach (string moduleId in _registeredModules)
        {
            IModule module = _serviceProvider.GetRequiredKeyedService<IModule>(moduleId);
            IBackendModuleSearchRegistrar registrar = (IBackendModuleSearchRegistrar)module;
            modules.Add(registrar);
        }

        ILoggerFactory loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        SearchProvider searchProvider = new(loggerFactory.CreateLogger<SearchProvider>(),
            modules);

        return searchProvider;
    }
}
