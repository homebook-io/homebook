using HomeBook.Backend.Core.Kitchen.Contracts;
using HomeBook.Backend.Core.Kitchen.Mappings;
using HomeBook.Backend.Core.Kitchen.Models;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Core.Kitchen;

/// <inheritdoc/>
public class RecipesProvider(
    ILogger<RecipesProvider> logger,
    IRecipesRepository recipesRepository) : IRecipesProvider
{
    /// <inheritdoc/>
    public async Task<RecipeDto[]> GetRecipesAsync(string searchFilter,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving meals with search filter: {SearchFilter}",
            searchFilter);

        IEnumerable<Recipe> recipeEntities = await recipesRepository.GetAsync(searchFilter,
            cancellationToken);
        RecipeDto[] recipes = recipeEntities
            .Select(m => m.ToDto())
            .ToArray();

        return recipes;
    }

    /// <inheritdoc/>
    public async Task<RecipeDto?> GetRecipeByIdAsync(Guid id,
        CancellationToken cancellationToken) =>
        (await recipesRepository.GetByIdAsync(id,
            cancellationToken))?.ToDto();

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(string name,
        CancellationToken cancellationToken)
    {
        Recipe entity = new()
        {
            Name = name
        };

        // TODO: validator

        Guid entityId = await recipesRepository
            .CreateOrUpdateAsync(entity,
                cancellationToken);
        return entityId;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        await recipesRepository.DeleteAsync(id,
            cancellationToken);

    /// <inheritdoc/>
    public async Task UpdateNameAsync(Guid id,
        string name,
        CancellationToken cancellationToken)
    {
        Recipe entity = await recipesRepository.GetByIdAsync(id,
                            cancellationToken)
                        ?? throw new KeyNotFoundException(
                            $"Recipe with id {id} not found");

        entity.Name = name;

        // TODO: validator

        await recipesRepository
            .CreateOrUpdateAsync(entity,
                cancellationToken);
        return;
    }
}
