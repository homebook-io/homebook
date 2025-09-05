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

        // 2. get latest update version
        // string? latestUpdateVersion = await setupInstanceManager.GetLatestUpdateVersionAsync(cancellationToken);
        //
        // // 3. execute all available updates
        // List<IUpdateMigrator> updateMigrators = availableUpdateMigrators
        //     .Where(um => string.IsNullOrEmpty(latestUpdateVersion) ||
        //                  string.Compare(um.Version, latestUpdateVersion, StringComparison.OrdinalIgnoreCase) > 0)
        //     .OrderBy(um => um.Version)
        //     .ToList();
        // try
        // {
        //     foreach (IUpdateMigrator updateMigrator in updateMigrators)
        //     {
        //         logger.LogInformation("Executing update: {Version} - {Description}",
        //             updateMigrator.Version,
        //             updateMigrator.Description);
        //         await updateMigrator.ExecuteAsync(cancellationToken);
        //     }
        // }
        // catch (Exception ex)
        // {
        //     logger.LogError(ex, "Error during update process, update aborted");
        //     throw new SetupException("Error during update process, update aborted");
        // }

        // 4. write current version to file
        await setupInstanceManager.CreateHomebookInstanceAsync(cancellationToken);
    }
}
