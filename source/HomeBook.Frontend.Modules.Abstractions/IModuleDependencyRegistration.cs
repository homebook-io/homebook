using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Frontend.Modules.Abstractions;

public interface IModuleDependencyRegistration
{
    /// <summary>
    /// register required services for this module.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    static abstract void RegisterServices(IServiceCollection services,
        IConfiguration configuration);
}
