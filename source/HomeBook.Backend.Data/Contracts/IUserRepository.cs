using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Data.Contracts;

/// <summary>
/// defines repository for user management
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// creates a new user with the given informations
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User> CreateUserAsync(User user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// checks if a user with the given username already exists
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ContainsUserAsync(string username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// returns the user with the given username or null if not found
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User?> GetUserByUsernameAsync(string username,
        CancellationToken cancellationToken = default);
}
