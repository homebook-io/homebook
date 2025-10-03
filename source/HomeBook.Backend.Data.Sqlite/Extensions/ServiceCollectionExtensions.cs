using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Data.Sqlite.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendDataSqliteProbe(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseProbe, SqliteProbe>();

        return services;
    }
    public static IServiceCollection AddBackendDataSqlite(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseManager, DatabaseManager>();

        // Initialize Database
        services.AddDbContextPool<AppDbContext>(optionsBuilder =>
            CreateDbContextOptionsBuilder(configuration, optionsBuilder));

        services.AddDbContextFactory<AppDbContext>(optionsBuilder =>
            CreateDbContextOptionsBuilder(configuration, optionsBuilder));

        return services;
    }

    public static void CreateDbContextOptionsBuilder(IConfiguration configuration,
        DbContextOptionsBuilder optionsBuilder)
    {
        string? file = configuration["Database:File"];

        string connectionString = ConnectionStringBuilder.Build(file!);

        optionsBuilder.SetDbOptions(connectionString);
    }
}
