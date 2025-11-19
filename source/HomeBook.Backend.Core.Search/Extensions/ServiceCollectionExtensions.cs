using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Search.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreSearch(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddSingleton<SearchRegistrationFactory>();
        services.AddSingleton<ISearchRegistrationFactory, SearchRegistrationFactory>(x => x.GetRequiredService<SearchRegistrationFactory>());
        services.AddSingleton<ISearchRegistrationInitiator, SearchRegistrationFactory>(x => x.GetRequiredService<SearchRegistrationFactory>());

        return services;
    }
}
