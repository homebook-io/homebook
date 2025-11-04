using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.Licenses.Extensions;
using Homebook.Backend.Core.Setup.Extensions;
using HomeBook.Backend.Core.HashProvider.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCore(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddBackendCoreSetup(configuration, instanceStatus)
            .AddBackendCoreLicenses(configuration, instanceStatus)
            .AddBackendCoreHashProvider(configuration, instanceStatus)
            .AddBackendCoreValidators(configuration, instanceStatus);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddBackendCoreValidators(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        return services;
    }
}
