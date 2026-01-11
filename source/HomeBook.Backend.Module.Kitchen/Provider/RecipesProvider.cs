using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.Module.Kitchen.Contracts;
using HomeBook.Backend.Module.Kitchen.Mappings;
using HomeBook.Backend.Module.Kitchen.Models;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Module.Kitchen.Provider;

/// <inheritdoc/>
public class RecipesProvider(
    ILogger<RecipesProvider> logger,
    IRecipesRepository recipesRepository,
    IIngredientRepository ingredientRepository) : IRecipesProvider
{
    /// <inheritdoc/>
    public async Task<RecipeResultDto[]> GetRecipesAsync(string searchFilter,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving meals with search filter: {SearchFilter}",
            searchFilter);

        IEnumerable<Recipe> recipeEntities = await recipesRepository.GetAsync(searchFilter,
            cancellationToken);
        RecipeResultDto[] recipes = recipeEntities
            .Select(m => m.ToDto())
            .ToArray();

        return recipes;
    }

    /// <inheritdoc/>
    public async Task<RecipeResultDto?> GetRecipeByIdAsync(Guid id,
        CancellationToken cancellationToken) =>
        (await recipesRepository.GetByIdAsync(id,
            cancellationToken))?.ToDto();

    /// <inheritdoc/>
    public async Task<Guid> CreateOrUpdateAsync(RecipeRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        // TODO: validate dto

        Recipe entity = requestDto.ToEntity();

        // TODO: validate entity

        Guid entityId = await recipesRepository
            .CreateOrUpdateAsync(entity,
                cancellationToken);
        return entityId;
    }

    /// <inheritdoc/>
    public async Task<RecipeIngredient?> GetIngredientByNameAsync(string name,
        CancellationToken cancellationToken) =>
        await ingredientRepository.GetByName(name,
            cancellationToken);

    /// <inheritdoc/>
    public async Task<Guid> CreateIngredientAsync(string name,
        CancellationToken cancellationToken)
    {
        RecipeIngredient entity = new()
        {
            Name = name
        };

        Guid entityId = await ingredientRepository.CreateOrUpdateAsync(entity,
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
        // Recipe entity = await recipesRepository.GetByIdAsync(id,
        //                     cancellationToken)
        //                 ?? throw new KeyNotFoundException(
        //                     $"Recipe with id {id} not found");
        //
        // entity.Name = name;

        // TODO: validator

        await recipesRepository.UpdateRecipeNameAsync(id,
            name,
            cancellationToken);
    }
}
