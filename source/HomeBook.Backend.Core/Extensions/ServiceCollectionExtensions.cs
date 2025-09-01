using HomeBook.Backend.Core.Licenses.Extensions;
using Homebook.Backend.Core.Setup.Extensions;
using HomeBook.Backend.Core.HashProvider.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCore(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddBackendCoreSetup(configuration)
            .AddBackendCoreLicenses(configuration)
            .AddBackendCoreHashProvider(configuration)
            .AddBackendCoreValidators(configuration);

        return services;
    }

    private static IServiceCollection AddBackendCoreValidators(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
