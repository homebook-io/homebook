using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Core.Kitchen.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Kitchen.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreKitchen(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddScoped<IRecipesProvider, RecipesProvider>();

        return services;
    }
}
