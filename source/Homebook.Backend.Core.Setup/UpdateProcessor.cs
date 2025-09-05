using HomeBook.Backend.Abstractions.Contracts;
using Homebook.Backend.Core.Setup.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup;

public class UpdateProcessor(
    ILogger<UpdateProcessor> logger,
    IConfiguration configuration,
    ISetupInstanceManager setupInstanceManager,
    IUpdateManager updateManager,
    IDatabaseMigratorFactory databaseMigratorFactory) : IUpdateProcessor
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        // 1. create directory structure ( to create new directories from this update)
        setupInstanceManager.CreateRequiredDirectories();

        // 2. execute databae migrations
        string? databaseType = configuration["Database:Provider"];
        if (string.IsNullOrEmpty(databaseType))
            throw new SetupException("database provider is not configured");

        try
        {
            logger.LogInformation("Starting database migration for provider: {DatabaseType}", databaseType);
            IDatabaseMigrator databaseMigrator = databaseMigratorFactory.CreateMigrator(databaseType);
            await databaseMigrator.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database migration, update aborted");
            throw new SetupException("Error during database migration, update aborted");
        }

        // 3. execute all available updates
        await updateManager.ExecuteAvailableUpdateAsync(cancellationToken);

        // 4. write current version to file
        await setupInstanceManager.CreateHomebookInstanceAsync(cancellationToken);
    }
}
