using HomeBook.Backend.Abstractions.Models;

namespace HomeBook.Backend.Abstractions.Contracts;

/// <summary>
/// Service for JWT token operations
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the given user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="username">The username</param>
    /// <returns>A JWT token result</returns>
    JwtTokenResult GenerateToken(Guid userId, string username);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>True if the token is valid, false otherwise</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Extracts the user ID from a JWT token
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>The user ID if valid, null otherwise</returns>
    Guid? GetUserIdFromToken(string token);
}
