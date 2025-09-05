using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models;
using Homebook.Backend.Core.Setup.Factories;
using Homebook.Backend.Core.Setup.Models;
using Homebook.Backend.Core.Setup.Provider;
using Homebook.Backend.Core.Setup.UpdateMigrators;
using Homebook.Backend.Core.Setup.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Homebook.Backend.Core.Setup.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreSetup(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddBackendCoreSetupEnvironment(configuration)
            .AddBackendCoreSetupValidators(configuration)
            .AddBackendCoreSetupUpdateComponents(configuration);

        services.AddSingleton<ISetupConfigurationProvider, SetupConfigurationProvider>();
        services.AddSingleton<ISetupInstanceManager, SetupInstanceManager>();
        services.AddScoped<ISetupProcessorFactory, SetupProcessorFactory>();

        return services;
    }
    public static IServiceCollection AddBackendCoreSetupUpdateComponents(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUpdateProcessor, UpdateProcessor>();
        services.AddScoped<IUpdateManager, UpdateManager>();

        // update migrators
        services.AddScoped<IUpdateMigrator, Update_1_0_10>();

        return services;
    }

    private static IServiceCollection AddBackendCoreSetupEnvironment(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IValidator<EnvironmentConfiguration>, EnvironmentValidator>();

        return services;
    }

    private static IServiceCollection AddBackendCoreSetupValidators(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IValidator<SetupConfiguration>, SetupConfigurationValidator>();

        return services;
    }
}
