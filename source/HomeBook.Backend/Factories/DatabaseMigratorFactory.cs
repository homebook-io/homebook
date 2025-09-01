using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Exceptions;

namespace HomeBook.Backend.Factories;

/// <inheritdoc />
public class DatabaseMigratorFactory(IConfiguration configuration) : IDatabaseMigratorFactory
{
    /// <inheritdoc />
    public IDatabaseMigrator CreateMigrator(string databaseType)
    {
        return databaseType.ToUpperInvariant() switch
        {
            "POSTGRESQL" => new Data.PostgreSql.DatabaseMigrator(configuration),
            "MYSQL" => new Data.Mysql.DatabaseMigrator(configuration),
            _ => throw new UnsupportedDatabaseException($"Unsupported database provider: {databaseType}")
        };
    }
}
