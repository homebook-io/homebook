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
        CancellationToken cancellationToken,
        AppDbContext? appDbContext = null)
    {
        if (appDbContext is null)
        {
            await using AppDbContext newDbContext = await factory.CreateDbContextAsync(cancellationToken);
            return await GetByIdAsync(entityId, cancellationToken, newDbContext);
        }

        Recipe? entity = await appDbContext.Set<Recipe>()
            .Include(r => r.Recipe2RecipeIngredient)
            .ThenInclude(ri => ri.RecipeIngredient)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Id == entityId, cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateOrUpdateAsync(Recipe entity,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        Recipe? existing = await GetByIdAsync(entity.Id,
            cancellationToken,
            dbContext);

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

    /// <inheritdoc />
    public async Task<Recipe2RecipeIngredient> CreateOrUpdateAsync(Recipe2RecipeIngredient entity,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        Recipe2RecipeIngredient? existing = await GetAsync(entity.RecipeId,
            entity.IngredientId,
            cancellationToken,
            dbContext);

        if (existing is null)
        {
            dbContext.Recipe2RecipeIngredients.Add(entity);
        }
        else
        {
            dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task<Recipe2RecipeIngredient?> GetAsync(Guid recipeId,
        Guid ingredientId,
        CancellationToken cancellationToken,
        AppDbContext? appDbContext = null)
    {
        if (appDbContext is null)
        {
            await using AppDbContext newDbContext = await factory.CreateDbContextAsync(cancellationToken);
            return await GetAsync(recipeId, ingredientId, cancellationToken, newDbContext);
        }

        Recipe2RecipeIngredient? entity = await appDbContext.Set<Recipe2RecipeIngredient>()
            .Include(r2ri => r2ri.RecipeIngredient)
            .Include(r2ri => r2ri.Recipe)
            .FirstOrDefaultAsync(r2ri => r2ri.RecipeId == recipeId
                                         && r2ri.IngredientId == ingredientId,
                cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<RecipeStep> CreateRecipeStepAsync(RecipeStep entity,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        dbContext.RecipeSteps.Add(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
