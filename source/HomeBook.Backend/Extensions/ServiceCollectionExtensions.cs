using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Exceptions;
using Homebook.Backend.Core.Setup.Provider;
using HomeBook.Backend.Data;
using HomeBook.Backend.Data.Extensions;
using HomeBook.Backend.Data.Mysql.Extensions;
using HomeBook.Backend.Data.PostgreSql.Extensions;
using HomeBook.Backend.Provider;
using HomeBook.Backend.Services;

namespace HomeBook.Backend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register the file service
        services.AddSingleton<IApplicationPathProvider, NativeFileService>();
        services.AddSingleton<IFileSystemService, NativeFileService>();

        // Register other services as needed
        services.AddSingleton<IRuntimeConfigurationProvider, RuntimeConfigurationProvider>(); // Program.cs

        return services;
    }

    public static IServiceCollection AddBackendDatabaseProvider(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseProviderResolver, DatabaseProviderResolver>();
        services.AddBackendDataPostgreSqlProbe(configuration);
        services.AddBackendDataMysqlProbe(configuration);

        // Get database provider from configuration
        string? databaseType = configuration["Database:Provider"];
        // TODO: breakpoint
        if (!string.IsNullOrEmpty((databaseType ?? string.Empty).Trim()))
        {
            // load database provider specific services
            switch (databaseType?.ToLowerInvariant())
            {
                case "postgresql":
                    services.AddBackendDataPostgreSql(configuration);
                    break;
                case "mysql":
                    services.AddBackendDataMysql(configuration);
                    break;
                default:
                    throw new UnsupportedDatabaseException($"Unsupported database provider: {databaseType}");
            }

            // load common database services (repositories, etc.)
            services.AddBackendData(configuration);
        }


        return services;
    }
}
