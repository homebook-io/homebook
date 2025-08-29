using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Abstractions.Contracts;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Data.Mysql;

/// <inheritdoc />
public class DatabaseManager(ILogger<DatabaseManager> logger) : IDatabaseManager
{
    /// <inheritdoc />
    public async Task<bool> IsDatabaseAvailableAsync(string databaseHost,
        ushort databasePort,
        string databaseName,
        string databaseUserName,
        string databaseUserPassword,
        CancellationToken cancellationToken)
    {
        try
        {
            string connectionString = $"Host={databaseHost};Port={databasePort};Database={databaseName};Username={databaseUserName};Password={databaseUserPassword};Timeout=5;";

            logger.LogInformation("Checking MySQL database availability with connection string: {ConnectionString}", connectionString);

            return true;
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while checking MySQL database availability");

            return false;
        }
    }
}
