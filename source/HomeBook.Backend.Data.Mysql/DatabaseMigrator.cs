using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Mysql.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Data.Mysql;

/// <inheritdoc />
public class DatabaseMigrator(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    IEnumerable<SaveChangesInterceptor> saveChangesInterceptors) : IDatabaseMigrator
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
        ServiceCollectionExtensions.CreateDbContextOptionsBuilder(configuration, serviceProvider, optionsBuilder);

        AppDbContext context = new(optionsBuilder.Options,
            saveChangesInterceptors);
        return context;
    }

    public void ConfigureForServiceCollection(ServiceCollection services, IConfiguration configuration)
    {
        services.AddBackendDataMysql(configuration);
    }
}
