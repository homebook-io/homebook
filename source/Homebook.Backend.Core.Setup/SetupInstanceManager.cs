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
    private readonly string _setupInstanceFileName = Path.Combine(applicationPathProvider.ConfigurationPath, ".setup");
    private readonly string _homebookInstanceFileName = Path.Combine(applicationPathProvider.ConfigurationPath, ".homebook");

    /// <inheritdoc />
    public bool IsSetupInstanceCreated()
    {
        logger.LogInformation("Checking if setup instance file exists at {FilePath}", _setupInstanceFileName);

        bool instanceFileExists = fileSystemService.FileExists(_setupInstanceFileName);

        return instanceFileExists; // true => means setup is already executed and instance is created
    }

    /// <inheritdoc />
    public async Task CreateSetupInstanceAsync(CancellationToken cancellationToken = default)
    {
        string instanceFilePath = Path.Combine(applicationPathProvider.DataDirectory, "instance.txt");
        logger.LogInformation("Write setup instance file at {FilePath}", instanceFilePath);

        string appVersion = configuration.GetSection("Version")?.Value?.Trim() ?? string.Empty;
        await fileSystemService.FileWriteAllTextAsync(instanceFilePath, appVersion, cancellationToken);
    }

    public void CreateRequiredDirectories()
    {
        string[] requiredDirectories =
        [
            applicationPathProvider.ConfigurationPath,
            applicationPathProvider.CacheDirectory,
            applicationPathProvider.LogDirectory,
            applicationPathProvider.DataDirectory,
            applicationPathProvider.TempDirectory
        ];

        foreach (string dir in requiredDirectories)
        {
            if (fileSystemService.DirectoryExists(dir))
                continue;

            logger.LogInformation("Creating required directory at {Directory}", dir);
            fileSystemService.CreateDirectory(dir);
        }
    }

    public async Task<bool> IsUpdateRequiredAsync(CancellationToken cancellationToken = default)
    {
        // get the version from the appsettings
        string? runningAppVersion = configuration.GetSection("Version")?.Value?.Trim();
        string? installedInstanceVersion = null;
        try
        {
            // get the version from the instance file
            installedInstanceVersion = await fileSystemService.FileReadAllTextAsync(_homebookInstanceFileName, cancellationToken);
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error reading instance version from file");
            return false;
        }

        if (string.IsNullOrEmpty(runningAppVersion)
            || string.IsNullOrEmpty(installedInstanceVersion))
            return false;

        int versionComparison = new Version(runningAppVersion).CompareTo(new Version(installedInstanceVersion));
        if (versionComparison <= 0)
            return false;

        return true;
    }

    public bool IsSetupFinishedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Checking if homebook instance file exists at {FilePath}", _homebookInstanceFileName);

        bool instanceFileExists = fileSystemService.FileExists(_homebookInstanceFileName);

        return instanceFileExists; // true => means setup is finished
    }
}
