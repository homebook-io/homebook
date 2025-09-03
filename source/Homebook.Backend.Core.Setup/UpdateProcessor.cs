using HomeBook.Backend.Abstractions.Contracts;
using Homebook.Backend.Core.Setup.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Homebook.Backend.Core.Setup;

public class UpdateProcessor(
    IConfiguration configuration,
    ISetupInstanceManager setupInstanceManager,
    IEnumerable<IUpdateMigrator> availableUpdateMigrators,
    IDatabaseMigratorFactory databaseMigratorFactory) : IUpdateProcessor
{
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        // 1. execute databae migrations
        string? databaseType = configuration["Database:Provider"];
        if (string.IsNullOrEmpty(databaseType))
            throw new SetupException("database provider is not configured");

        IDatabaseMigrator databaseMigrator = databaseMigratorFactory.CreateMigrator(databaseType);
        await databaseMigrator.MigrateAsync(cancellationToken);

        // 2. get latest update version
        string? latestUpdateVersion = await setupInstanceManager.GetLatestUpdateVersionAsync(cancellationToken);

        // 3. execute all available updates
        List<IUpdateMigrator> updateMigrators = availableUpdateMigrators
            .Where(um => string.IsNullOrEmpty(latestUpdateVersion) ||
                         string.Compare(um.Version, latestUpdateVersion, StringComparison.OrdinalIgnoreCase) > 0)
            .OrderBy(um => um.Version)
            .ToList();
        foreach (IUpdateMigrator updateMigrator in updateMigrators)
        {
            await updateMigrator.ExecuteAsync(cancellationToken);
        }

        // 4. write current version to file
        await setupInstanceManager.CreateHomebookInstanceAsync(cancellationToken);
    }
}
