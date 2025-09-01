using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.EnvironmentHandler;

namespace HomeBook.Backend.Services;

public class NativeFileService(ILogger<NativeFileService> logger) : IApplicationPathProvider, IFileSystemService
{
    public string ConfigurationPath { get; } = PathHandler.ConfigurationPath;
    public string RuntimeConfigurationFilePath { get; } = PathHandler.RuntimeConfigurationFilePath;
    public string CacheDirectory { get; } = PathHandler.CacheDirectory;
    public string LogDirectory { get; } = PathHandler.LogDirectory;
    public string DataDirectory { get; } = PathHandler.DataDirectory;
    public string TempDirectory { get; } = PathHandler.TempDirectory;

    /// <inheritdoc />
    public bool FileExists(string path)
    {
        logger.LogInformation("Checking if file exists: {Path}", path);

        return File.Exists(path);
    }

    /// <inheritdoc />
    public async Task<string> FileReadAllTextAsync(string path, CancellationToken cancellationToken) => await File.ReadAllTextAsync(path, cancellationToken);

    /// <inheritdoc />
    public async Task FileWriteAllTextAsync(string path, string content, CancellationToken cancellationToken) => await File.WriteAllTextAsync(path, content, cancellationToken);

    /// <inheritdoc />
    public bool DirectoryExists(string path) => Directory.Exists(path);

    /// <inheritdoc />
    public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
}


