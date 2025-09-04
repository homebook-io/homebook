using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    /// <inheritdoc />
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    /// <inheritdoc />
    public async Task<bool> ContainsUserAsync(string username, CancellationToken cancellationToken = default) =>
        (await GetUserByUsernameAsync(username, cancellationToken)) is not null;

    /// <inheritdoc />
    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Users.ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}
