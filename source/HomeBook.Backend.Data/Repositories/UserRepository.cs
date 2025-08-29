using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class UserRepository(
    AppDbContext dbContext
    // ,
    // IRepository<User> repository
) : IUserRepository
{
    /// <inheritdoc />
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        // await repository.AddAsync(user, cancellationToken);
        // await repository.SaveChangesAsync(cancellationToken);

        return user;
    }

    /// <inheritdoc />
    public async Task<bool> ContainsUserAsync(string username, CancellationToken cancellationToken = default) =>
        (await GetUserByUsernameAsync(username, cancellationToken)) is not null;

    // /// <inheritdoc />
    // public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
    //     await repository.GetAsync<User>(x => x.Username == username, cancellationToken);

    /// <inheritdoc />
    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        User? user = await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        return user;
    }
}
