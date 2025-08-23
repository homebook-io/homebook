using FluentValidation;
using HomeBook.Backend.Core.Models;
using HomeBook.Backend.Core.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCore(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddBackendCoreValidators(configuration);

        return services;
    }

    private static IServiceCollection AddBackendCoreValidators(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IValidator<DatabaseConfiguration>, DatabaseConfigurationValidator>();

        return services;
    }
}
