using HomeBook.Backend.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Data.Mysql.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendDataMysqlProbe(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseProbe, MysqlProbe>();

        return services;
    }
    public static IServiceCollection AddBackendDataMysql(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseManager, DatabaseManager>();

        return services;
    }
}
