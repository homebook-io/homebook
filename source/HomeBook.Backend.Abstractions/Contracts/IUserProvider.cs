namespace HomeBook.Backend.Abstractions.Contracts;

/// <summary>
/// contains provider definitions for user management
/// </summary>
public interface IUserProvider
{
    /// <summary>
    /// creates a new user with the given informations
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateUserAsync(string username,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// returns if a user with the given username already exists
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ContainsUserAsync(string username,
        CancellationToken cancellationToken = default);
}
