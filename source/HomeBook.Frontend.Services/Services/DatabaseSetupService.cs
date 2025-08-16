using HomeBook.Client;
using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;

namespace HomeBook.Frontend.Services.Services;

/// <inheritdoc />
public class DatabaseSetupService(
    ILogger<DatabaseSetupService> logger,
    BackendClient backendClient) : IDatabaseSetupService
{
    /// <inheritdoc />
    public async Task<bool> CheckConnectionAsync(string host,
        ushort port,
        string databaseName,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        try
        {
            await backendClient.Setup.Database.Check.PostAsync(
                new CheckDatabaseRequest
                {
                    DatabaseHost = host,
                    DatabasePort = port,
                    DatabaseName = databaseName,
                    DatabaseUserName = username,
                    DatabaseUserPassword = password
                },
                x =>
                {
                },
                cancellationToken);

            return true;
        }
        catch (ApiException err) when (err.ResponseStatusCode == 500)
        {
            logger.LogWarning("Database connection check failed: {Message}", err.Message);

            return false;
        }
        catch (ApiException err) when (err.ResponseStatusCode == 503)
        {
            logger.LogWarning("Database connection check failed: {Message}", err.Message);

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database connection check");

            return false;
        }
    }
}
