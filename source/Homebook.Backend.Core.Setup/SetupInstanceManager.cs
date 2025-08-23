using HomeBook.Backend.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Homebook.Backend.Core.Setup;

/// <inheritdoc />
public class SetupInstanceManager(
    ILogger<SetupInstanceManager> logger,
    IConfiguration configuration,
    IFileSystemService fileSystemService,
    IApplicationPathProvider applicationPathProvider) : ISetupInstanceManager
{
    /// <inheritdoc />
    public bool IsSetupInstanceCreated()
    {
        string instanceFilePath = Path.Combine(applicationPathProvider.DataDirectory, "instance.txt");
        logger.LogInformation("Checking if setup instance file exists at {FilePath}", instanceFilePath);

        bool instanceFileExists = fileSystemService.FileExists(instanceFilePath);

        return instanceFileExists; // true => means setup is already executed and instance is created
    }

    /// <inheritdoc />
    public async Task CreateSetupInstanceAsync(CancellationToken cancellationToken = default)
    {
        string instanceFilePath = Path.Combine(applicationPathProvider.DataDirectory, "instance.txt");
        logger.LogInformation("Write setup instance file at {FilePath}", instanceFilePath);

        string appVersion = configuration["Version"] ?? string.Empty;
        await fileSystemService.FileWriteAllTextAsync(instanceFilePath, appVersion, cancellationToken);
    }
}
