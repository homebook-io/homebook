using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Data.Contracts;

/// <summary>
/// defines the contract for configuration repository
/// </summary>
public interface IConfigurationRepository
{
    /// <summary>
    /// writes a configuration key-value pair
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteConfigurationAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
