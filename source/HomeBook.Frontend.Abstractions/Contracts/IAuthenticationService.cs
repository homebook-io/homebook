using System.Security.Claims;

namespace HomeBook.Frontend.Abstractions.Contracts;

/// <summary>
/// defines methods for user authentication and session management.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// logs in a user with the provided username and password.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// logs out the current user and clears authentication tokens.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// checks if a user is currently authenticated.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// returns the current authentication token, if available.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// returns the current user's claims principal.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// event triggered when the authentication state changes (e.g., login or logout).
    /// </summary>
    event Action<bool>? AuthenticationStateChanged;
}
