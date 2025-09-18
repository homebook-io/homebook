using FluentValidation;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.DataProvider.Validators;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.DataProvider.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreDataProvider(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddScoped<IUserProvider, UserProvider>();
        services.AddScoped<IInstanceConfigurationProvider, InstanceConfigurationProvider>();

        return services;
    }

    public static IServiceCollection AddBackendCoreDataProviderValidators(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddSingleton<IValidator<User>, UserValidator>();
        services.AddSingleton<IValidator<Configuration>, ConfigurationValidator>();

        return services;
    }
}
