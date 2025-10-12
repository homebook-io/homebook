using HomeBook.Frontend.Abstractions.Enums;

namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
/// defines a service that is run on application startup.
/// </summary>
public interface IStartupService
{
    /// <summary>
    /// event triggered when the authentication state changes (e.g., login or logout).
    /// </summary>
    event Action<AppStatus>? ApplicationInitialized;

    /// <summary>
    /// the current status of the app.
    /// </summary>
    AppStatus Status { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(CancellationToken cancellationToken);
}
