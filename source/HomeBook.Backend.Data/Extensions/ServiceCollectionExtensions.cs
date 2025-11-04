using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendData(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<ISavingGoalsRepository, SavingGoalsRepository>();

        return services;
    }
}
