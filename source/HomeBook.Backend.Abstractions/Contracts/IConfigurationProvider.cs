namespace HomeBook.Backend.Abstractions.Contracts;

/// <summary>
/// defines the configuration provider
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// writes the homebook instance name to the configuration
    /// </summary>
    /// <param name="instanceName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteHomeBookInstanceNameAsync(string instanceName, CancellationToken cancellationToken = default);
}
