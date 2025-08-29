using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.HashProvider.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreHashProvider(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IHashProviderFactory, HashProviderFactory>();

        return services;
    }
}
