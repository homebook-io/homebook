using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Exceptions;

namespace HomeBook.Backend.Factories;

/// <inheritdoc />
public class DatabaseMigratorFactory(IServiceProvider serviceProvider)
    : IDatabaseMigratorFactory
{
    /// <inheritdoc />
    public IDatabaseMigrator CreateMigrator(string databaseType)
    {
        string key = databaseType.ToUpperInvariant();
        IDatabaseMigrator migrator = serviceProvider.GetKeyedService<IDatabaseMigrator>(key)
                                     ?? throw new UnsupportedDatabaseException(
                                         $"Unsupported database provider: {databaseType}");

        return migrator;
    }
}
