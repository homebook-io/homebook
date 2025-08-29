using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Core.DataProvider.UserManagement;
using HomeBook.Backend.Core.DataProvider.Validators;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfigurationProvider = HomeBook.Backend.Abstractions.Contracts.IConfigurationProvider;

namespace HomeBook.Backend.Core.DataProvider.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreDataProvider(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserProvider, UserProvider>();
        services.AddScoped<IConfigurationProvider, ConfigurationProvider>();

        return services;
    }

    public static IServiceCollection AddBackendCoreDataProviderValidators(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IValidator<User>, UserValidator>();
        services.AddSingleton<IValidator<Configuration>, ConfigurationValidator>();

        return services;
    }
}
