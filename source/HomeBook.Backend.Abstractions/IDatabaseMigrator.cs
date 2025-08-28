namespace HomeBook.Backend.Abstractions;

/// <summary>
/// definition for a database migrator
/// </summary>
public interface IDatabaseMigrator
{
    /// <summary>
    /// migrate the database to the latest version
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
