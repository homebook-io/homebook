using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class UserPreferenceRepository(
    ILogger<UserPreferenceRepository> logger,
    IDbContextFactory<AppDbContext> factory) : IUserPreferenceRepository
{
    /// <inheritdoc />
    public async Task<UserPreference?> GetPreferenceForUserByKeyAsync(Guid userId,
        string key,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        UserPreference? userPreference = await dbContext.Set<UserPreference>()
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        return userPreference;
    }

    /// <inheritdoc />
    public async Task SetPreferenceForUserByKeyAsync(UserPreference preference,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        UserPreference? existingPreference = await dbContext.Set<UserPreference>()
            .FirstOrDefaultAsync(c => c.UserId == preference.UserId
                                      && c.Key == preference.Key,
                cancellationToken);

        if (existingPreference is null)
        {
            dbContext.Add(preference);
        }
        else
        {
            existingPreference.Value = preference.Value;
            dbContext.Update(existingPreference);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
