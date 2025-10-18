using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Core.Finances.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Finances.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreFinances(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddScoped<ISavingGoalsProvider, SavingGoalsProvider>();

        return services;
    }
}
