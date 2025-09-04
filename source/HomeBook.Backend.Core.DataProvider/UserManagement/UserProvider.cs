using FluentValidation;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Abstractions.Models.UserManagement;
using HomeBook.Backend.Core.DataProvider.Mappings;
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
            // TODO: replace with custom exception
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

        await userRepository.CreateUserAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ContainsUserAsync(string username, CancellationToken cancellationToken = default) =>
        await userRepository.ContainsUserAsync(username, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<User> userEntities = await userRepository.GetAllAsync(cancellationToken);
        IEnumerable<UserInfo> userInfos = userEntities.Select(x => x.ToUserInfo());
        return userInfos;
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(UserInfo userInfo,
        CancellationToken cancellationToken)
    {
        User user = await userRepository.GetUserByIdAsync(userInfo.Id, cancellationToken)
                    ?? throw new KeyNotFoundException($"User with id '{userInfo.Id}' not found.");
        user = user.Update(userInfo);

        await userValidator.ValidateAndThrowAsync(user, cancellationToken);

        await userRepository.UpdateUserAsync(user, cancellationToken);
    }
}
