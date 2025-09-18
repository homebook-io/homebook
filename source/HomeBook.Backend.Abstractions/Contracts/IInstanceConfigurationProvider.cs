namespace HomeBook.Backend.Abstractions.Contracts;

/// <summary>
/// defines the configuration provider
/// </summary>
public interface IInstanceConfigurationProvider
{
    /// <summary>
    /// writes the homebook instance name to the configuration
    /// </summary>
    /// <param name="instanceName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteHomeBookInstanceNameAsync(string instanceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// reads the homebook instance name from the configuration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>the instance name or null if not set</returns>
    Task<string> GetHomeBookInstanceNameAsync(CancellationToken cancellationToken = default);
}
