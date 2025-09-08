using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Core.Account;

/// <inheritdoc />
public class AccountProvider(
    IUserRepository userRepository,
    IHashProviderFactory hashProviderFactory,
    IJwtService jwtService,
    ILogger<AccountProvider> logger)
    : IAccountProvider
{
    /// <inheritdoc />
    public async Task<JwtTokenResult?> LoginAsync(string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(hashProviderFactory);
        ArgumentNullException.ThrowIfNull(jwtService);
        ArgumentNullException.ThrowIfNull(logger);

        try
        {
            // Benutzer aus der Datenbank laden
            User? user = await userRepository.GetUserByUsernameAsync(username, cancellationToken);
            if (user == null)
            {
                logger.LogWarning("Login failed: User '{Username}' not found", username);
                return null;
            }

            // Prüfen ob der Benutzer deaktiviert ist
            if (user.Disabled.HasValue)
            {
                logger.LogWarning("Login failed: User '{Username}' is disabled", username);
                return null;
            }

            // Hash-Provider basierend auf dem gespeicherten Hash-Typ erstellen
            if (!hashProviderFactory.IsSupported(user.PasswordHashType))
            {
                logger.LogError("Unsupported hash algorithm '{HashType}' for user '{Username}'",
                    user.PasswordHashType,
                    username);
                return null;
            }

            IHashProvider hashProvider = hashProviderFactory.Create(user.PasswordHashType);

            // Passwort verifizieren
            if (!hashProvider.Verify(password, user.PasswordHash))
            {
                logger.LogWarning("Login failed: Invalid password for user '{Username}'", username);
                return null;
            }

            // JWT Token generieren
            JwtTokenResult tokenResult = jwtService.GenerateToken(user.Id, user.Username);

            logger.LogInformation("Login successful for user '{Username}'", username);
            return tokenResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user '{Username}'", username);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jwtService);
        ArgumentNullException.ThrowIfNull(logger);

        try
        {
            // Token validieren
            if (!jwtService.ValidateToken(token))
            {
                logger.LogWarning("Logout failed: Invalid token");
                return false;
            }

            // TODO: Hier könnte eine Token-Blacklist implementiert werden
            // um Tokens vor Ablauf ungültig zu machen
            await Task.CompletedTask;

            logger.LogInformation("Logout successful");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            return false;
        }
    }
}
