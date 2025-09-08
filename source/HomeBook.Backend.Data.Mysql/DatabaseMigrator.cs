using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Mysql.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Data.Mysql;

/// <inheritdoc />
public class DatabaseMigrator(IConfiguration configuration) : IDatabaseMigrator
{
    /// <inheritdoc />
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await using AppDbContext context = (AppDbContext)GetDbContext();
        await context.Database.MigrateAsync(cancellationToken);
    }

    /// <inheritdoc />
    public DbContext GetDbContext()
    {
        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();
        ServiceCollectionExtensions.CreateDbContextOptionsBuilder(configuration, optionsBuilder);

        AppDbContext context = new(optionsBuilder.Options);
        return context;
    }

    public void ConfigureForServiceCollection(ServiceCollection services, IConfiguration configuration)
    {
        services.AddBackendDataMysql(configuration);
    }
}
