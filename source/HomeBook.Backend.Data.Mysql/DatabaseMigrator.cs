using HomeBook.Backend.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HomeBook.Backend.Data.Mysql;

/// <inheritdoc />
public class DatabaseMigrator(IConfiguration configuration) : IDatabaseMigrator
{
    /// <inheritdoc />
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        string? host = configuration["Database:Host"];
        string? port = configuration["Database:Port"];
        string? database = configuration["Database:InstanceDbName"];
        string? username = configuration["Database:Username"];
        string? password = configuration["Database:Password"];

        string connectionString = ConnectionStringBuilder.Build(host!, port!, database!, username!, password!);

        DbContextOptionsBuilder<AppDbContextCore> optionsBuilder = new();
        optionsBuilder.SetDbOptions(connectionString);

        await using AppDbContext contextBase = new(optionsBuilder.Options);
        await contextBase.Database.MigrateAsync(cancellationToken);
    }
}
