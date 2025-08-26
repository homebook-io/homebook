namespace HomeBook.Backend.Abstractions;

/// <summary>
/// contract for managing the setup instance of the application.
/// </summary>
public interface ISetupInstanceManager
{
    /// <summary>
    /// Checks if a setup instance has been created.
    /// </summary>
    /// <returns></returns>
    bool IsSetupInstanceCreated();

    /// <summary>
    /// Creates a setup instance by writing the application version to a file.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateSetupInstanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// creates the required directories for the application to function properly.
    /// </summary>
    /// <returns></returns>
    void CreateRequiredDirectories();

    /// <summary>
    /// Checks if an update is required by comparing the current application version with the version stored in the setup instance file.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsUpdateRequiredAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the setup process has been completed.
    /// </summary>
    /// <returns></returns>
    bool IsSetupFinishedAsync(CancellationToken cancellationToken = default);
}
