using HomeBook.Backend.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Data.PostgreSql.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendDataPostgreSql(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseManager, DatabaseManager>();

        return services;
    }
}
