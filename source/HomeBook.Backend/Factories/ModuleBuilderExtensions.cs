using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.ModuleCore;

namespace HomeBook.Backend.Factories;

public static class ModuleBuilderExtensions
{
    public static void RegisterModulesInSearchFactory(this ModuleBuilder moduleBuilder,
        ISearchRegistrationInitiator searchRegistrationInitiator)
    {
        foreach (string moduleId in moduleBuilder.SearchEnabledModules)
        {
            searchRegistrationInitiator.AddModule(moduleId);
        }
    }
}
