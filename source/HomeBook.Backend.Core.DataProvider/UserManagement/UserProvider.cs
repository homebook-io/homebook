using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Core.DataProvider.UserManagement;

/// <inheritdoc />
public class UserProvider(
    IUserRepository userRepository,
    IHashProviderFactory hashProviderFactory,
    IValidator<User> userValidator) : IUserProvider
{
    /// <inheritdoc />
    public async Task CreateUserAsync(string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        bool userExists = await userRepository.ContainsUserAsync(username, cancellationToken);
        if (userExists)
            throw new InvalidOperationException($"User with username '{username}' already exists.");

        IHashProvider hashProvider = hashProviderFactory.CreateDefault();
        string hashAlgorithm = hashProvider.AlgorithmName;
        string hashedPassword = hashProvider.Hash(password);
        User user = new()
        {
            Username = username,
            PasswordHashType = hashAlgorithm,
            PasswordHash = hashedPassword,
        };

        await userValidator.ValidateAndThrowAsync(user, cancellationToken);

        User createdUser = await userRepository.CreateUserAsync(user, cancellationToken);
        int i = 0;
    }

    /// <inheritdoc />
    public async Task<bool> ContainsUserAsync(string username,
        CancellationToken cancellationToken = default)
    {
        return await userRepository.ContainsUserAsync(username, cancellationToken);
    }
}
