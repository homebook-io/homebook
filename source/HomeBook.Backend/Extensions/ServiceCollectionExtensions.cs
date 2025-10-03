using FluentValidation;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Exceptions;
using HomeBook.Backend.Abstractions.Models;
using HomeBook.Backend.Core.DataProvider.Extensions;
using HomeBook.Backend.Core.DataProvider.Validators;
using HomeBook.Backend.Core.HashProvider;
using HomeBook.Backend.Core.Licenses;
using Homebook.Backend.Core.Setup;
using Homebook.Backend.Core.Setup.Extensions;
using Homebook.Backend.Core.Setup.Factories;
using Homebook.Backend.Core.Setup.Models;
using Homebook.Backend.Core.Setup.Provider;
using Homebook.Backend.Core.Setup.Validators;
using HomeBook.Backend.Data;
using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.Data.Extensions;
using HomeBook.Backend.Data.Mysql.Extensions;
using HomeBook.Backend.Data.PostgreSql.Extensions;
using HomeBook.Backend.Data.Sqlite.Extensions;
using HomeBook.Backend.Factories;
using HomeBook.Backend.Provider;
using HomeBook.Backend.Services;

namespace HomeBook.Backend.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// add ALL services required for setup mode
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="instanceStatus"></param>
    /// <returns></returns>
    public static IServiceCollection AddBackendSetup(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        // validators
        services.AddSingleton<IValidator<SetupConfiguration>, SetupConfigurationValidator>();
        services.AddSingleton<IValidator<EnvironmentConfiguration>, EnvironmentValidator>();
        services.AddSingleton<IValidator<User>, UserValidator>();
        services.AddSingleton<IValidator<Configuration>, ConfigurationValidator>();

        // basic dependencies
        services.AddSingleton<IFileSystemService, NativeFileService>();
        services.AddSingleton<IApplicationPathProvider, NativeFileService>();
        services.AddSingleton<IRuntimeConfigurationProvider, RuntimeConfigurationProvider>();
        services.AddSingleton<IHashProviderFactory, HashProviderFactory>();
        services.AddSingleton<ILicenseProvider, LicenseProvider>();

        // setup dependencies
        services.AddSingleton<ISetupInstanceManager, SetupInstanceManager>();
        services.AddSingleton<ISetupConfigurationProvider, SetupConfigurationProvider>();
        services.AddScoped<ISetupProcessorFactory, SetupProcessorFactory>();

        // database dependencies
        services.AddBackendDatabaseProbes(configuration, instanceStatus);
        services.AddBackendDatabaseMigrators(configuration, instanceStatus);
        services.AddSingleton<IDatabaseMigratorFactory, DatabaseMigratorFactory>();

        return services;
    }

    public static IServiceCollection AddBackendServices(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        // Register the file service
        services.AddSingleton<IApplicationPathProvider, NativeFileService>();
        services.AddSingleton<IFileSystemService, NativeFileService>();
        services.AddBackendDatabaseMigrators(configuration, instanceStatus);
        services.AddSingleton<IDatabaseMigratorFactory, DatabaseMigratorFactory>();

        // Register other services as needed
        services.AddSingleton<IRuntimeConfigurationProvider, RuntimeConfigurationProvider>();

        return services;
    }

    public static IServiceCollection AddBackendDatabaseMigrators(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddKeyedSingleton<IDatabaseMigrator, Data.PostgreSql.DatabaseMigrator>("POSTGRESQL");
        services.AddKeyedSingleton<IDatabaseMigrator, Data.Mysql.DatabaseMigrator>("MYSQL");
        services.AddKeyedSingleton<IDatabaseMigrator, Data.Sqlite.DatabaseMigrator>("SQLITE");

        return services;
    }

    public static IServiceCollection AddBackendDatabaseProbes(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddSingleton<IDatabaseProviderResolver, DatabaseProviderResolver>();
        services.AddBackendDataPostgreSqlProbe(configuration);
        services.AddBackendDataMysqlProbe(configuration);
        services.AddBackendDataSqliteProbe(configuration);

        return services;
    }

    public static IServiceCollection AddBackendDatabaseProvider(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddBackendDatabaseProbes(configuration, instanceStatus);

        // Get database provider from configuration
        string? databaseType = configuration["Database:Provider"];
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
                case "sqlite":
                    services.AddBackendDataSqlite(configuration);
                    break;
                default:
                    throw new UnsupportedDatabaseException($"Unsupported database provider: {databaseType}");
            }

            // load common database services (repositories, etc.)
            services.AddBackendData(configuration, instanceStatus)
                .AddBackendCoreDataProvider(configuration, instanceStatus);
        }

        services.AddBackendCoreDataProviderValidators(configuration, instanceStatus);

        return services;
    }
}
