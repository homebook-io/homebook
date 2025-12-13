using System.Linq.Expressions;
using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
            dbContext.Add(entity);
        }
        else
        {
            await dbContext.Recipes
                .Where(u => u.Id == entity.Id)
                .ExecuteUpdateAsync(UpdateEntityProperties(entity),
                    cancellationToken: cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    private static Expression<Func<SetPropertyCalls<Recipe>, SetPropertyCalls<Recipe>>>
        UpdateEntityProperties(Recipe entity)
    {
        return s => s
            .SetProperty(u => u.Name, entity.Name)
            .SetProperty(u => u.NormalizedName, entity.NormalizedName)
            .SetProperty(u => u.Description, entity.Description)
            .SetProperty(u => u.DurationWorkingMinutes, entity.DurationWorkingMinutes)
            .SetProperty(u => u.DurationCookingMinutes, entity.DurationCookingMinutes)
            .SetProperty(u => u.DurationRestingMinutes, entity.DurationRestingMinutes)
            .SetProperty(u => u.CaloriesKcal, entity.CaloriesKcal)
            .SetProperty(u => u.Servings, entity.Servings)
            .SetProperty(u => u.Comments, entity.Comments)
            .SetProperty(u => u.Source, entity.Source)
            .SetProperty(u => u.NormalizedName, entity.NormalizedName);
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
            dbContext.Add(entity);
        }
        else
        {
            await dbContext.Recipe2RecipeIngredients
                .Where(u => u.RecipeId == entity.RecipeId
                            && u.IngredientId == entity.IngredientId)
                .ExecuteUpdateAsync(UpdateEntityProperties(entity),
                    cancellationToken: cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    private static Expression<
            Func<SetPropertyCalls<Recipe2RecipeIngredient>, SetPropertyCalls<Recipe2RecipeIngredient>>>
        UpdateEntityProperties(Recipe2RecipeIngredient entity)
    {
        return s => s
            .SetProperty(u => u.Quantity, entity.Quantity)
            .SetProperty(u => u.Unit, entity.Unit);
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
