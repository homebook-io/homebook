using HomeBook.Frontend.Abstractions.Enums;

namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
/// defines a service that is run on application startup.
/// </summary>
public interface IStartupService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(InstanceStatus instanceStatus,
        CancellationToken cancellationToken);
}
