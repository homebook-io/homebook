using System.Text.Json;
using HomeBook.Backend.Abstractions.Contracts;
using Homebook.Backend.Core.Setup.Exceptions;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup;

/// <inheritdoc />
public class UpdateManager(
    ILogger<UpdateManager> logger,
    IEnumerable<IUpdateMigrator> availableUpdateMigrators,
    IApplicationPathProvider applicationPathProvider,
    IFileSystemService fileSystemService) : IUpdateManager
{
    private string _updateMigrationIndex => Path.Combine(applicationPathProvider.UpdateDirectory, "updates.json");

    /// <inheritdoc />
    public async Task<Dictionary<Version, string>> GetAvailableUpdatesAsync(CancellationToken cancellationToken =
        default)
    {
        IEnumerable<IUpdateMigrator> pendingUpdates = await GetPendingUpdatesAsync(cancellationToken);
        return pendingUpdates.ToDictionary(um => um.Version, um => um.Description);
    }

    /// <inheritdoc />
    public async Task ExecuteAvailableUpdateAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<IUpdateMigrator> pendingUpdates = await GetPendingUpdatesAsync(cancellationToken);

        try
        {
            foreach (IUpdateMigrator updateMigrator in pendingUpdates)
            {
                logger.LogInformation("Executing update: {Version} - {Description}",
                    updateMigrator.Version,
                    updateMigrator.Description);
                await updateMigrator.ExecuteAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during update process, update aborted");
            throw new SetupException("Error during update process, update aborted");
        }
    }

    private async Task<IEnumerable<IUpdateMigrator>> GetPendingUpdatesAsync(CancellationToken cancellationToken =
        default)
    {
        string updateIndexContent = string.Empty;
        if (fileSystemService.FileExists(_updateMigrationIndex))
            updateIndexContent = await fileSystemService.FileReadAllTextAsync(_updateMigrationIndex, cancellationToken);

        Version[] appliedUpdates = string.IsNullOrEmpty(updateIndexContent)
            ? []
            : JsonSerializer.Deserialize<Version[]>(updateIndexContent) ?? [];

        List<IUpdateMigrator> updateMigrators = availableUpdateMigrators
            .OrderBy(um => um.Version)
            .ToList();

        // remove appliedUpdates from updateMigrators
        updateMigrators.RemoveAll(um => appliedUpdates.Contains(um.Version));

        return updateMigrators;
    }
}
