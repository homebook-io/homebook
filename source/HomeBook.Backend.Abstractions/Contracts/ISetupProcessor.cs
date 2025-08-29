using Microsoft.Extensions.Configuration;

namespace HomeBook.Backend.Abstractions.Contracts;

/// <summary>
/// defines a processor for handling setup operations
/// </summary>
public interface ISetupProcessor
{
    /// <summary>
    /// processes the setup
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="homebookUserName"></param>
    /// <param name="homebookUserPassword"></param>
    /// <param name="homebookConfigurationName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ProcessAsync(IConfiguration configuration,
        string? homebookUserName,
        string? homebookUserPassword,
        string? homebookConfigurationName,
        CancellationToken cancellationToken = default);
}
