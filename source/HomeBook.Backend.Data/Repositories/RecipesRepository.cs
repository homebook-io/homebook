using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class RecipesRepository(
    IDbContextFactory<AppDbContext> factory,
    IStringNormalizer stringNormalizer)
    : IRecipesRepository
{
    public async Task<IEnumerable<Recipe>> GetAsync(string? searchFilter,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        // return all without filter
        if (string.IsNullOrWhiteSpace(searchFilter))
            return await dbContext.Set<Recipe>()
                .ToListAsync(cancellationToken);

        // return with filter
        string normalizedFilter = stringNormalizer.Normalize(searchFilter);
        return await dbContext.Set<Recipe>()
            .Where(e => e.NormalizedName.Contains(normalizedFilter))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Recipe?> GetByIdAsync(Guid entityId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        Recipe? entity = await dbContext.Set<Recipe>()
            .Where(e => e.Id == entityId)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateOrUpdateAsync(Recipe entity,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        Recipe? existing = await GetByIdAsync(entity.Id,
            cancellationToken);

        if (existing is null)
        {
            dbContext.Recipes.Add(entity);
        }
        else
        {
            dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid entityId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<Recipe>()
            .Where(e => e.Id == entityId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
