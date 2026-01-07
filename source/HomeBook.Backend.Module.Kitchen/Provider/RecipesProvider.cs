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
    public async Task<Guid> CreateOrUpdateAsync(Guid? id,
        string name,
        Guid userId,
        string? description,
        int? servings,
        int? durationWorkingMinutes,
        int? durationCookingMinutes,
        int? durationRestingMinutes,
        int? caloriesKcal,
        string? comments,
        string? source,
        CancellationToken cancellationToken)
    {
        Recipe? existing = null;
        if (id.HasValue)
            existing = await recipesRepository.GetByIdAsync(id.Value, cancellationToken);

        Recipe entity = existing ?? new Recipe
        {
            UserId = userId,
            Name = name,
            NormalizedName = name,
            Description = description,
            DurationWorkingMinutes = durationWorkingMinutes,
            DurationCookingMinutes = durationCookingMinutes,
            DurationRestingMinutes = durationRestingMinutes,
            CaloriesKcal = caloriesKcal,
            Servings = servings,
            Comments = comments,
            Source = source
        };

        // TODO: validator

        Guid entityId = await recipesRepository
            .CreateOrUpdateAsync(entity,
                cancellationToken);
        return entityId;
    }

    /// <inheritdoc/>
    public async Task<Recipe2RecipeIngredient> AddIngredientToRecipeAsync(Guid recipeId,
        string name,
        double? quantity,
        string? unit,
        CancellationToken cancellationToken)
    {
        string normalizedName = name; // TODO: normalize
        RecipeIngredient? ingredient = await GetIngredientByNameAsync(name,
            cancellationToken);

        Guid? ingredientId = ingredient?.Id ?? await CreateIngredientAsync(name,
            cancellationToken);

        Recipe2RecipeIngredient entity = new()
        {
            RecipeId = recipeId,
            IngredientId = ingredientId!.Value,
            Quantity = quantity,
            Unit = unit
        };

        // TODO: validator

        logger.LogInformation("Adding ingredient {IngredientName} to recipe {RecipeId}",
            name,
            recipeId);

        Recipe2RecipeIngredient updatedEntity = await recipesRepository
            .CreateOrUpdateAsync(entity,
                cancellationToken);
        return updatedEntity;
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
    public async Task<RecipeStep> AddStepToRecipeAsync(Guid recipeId,
        int position,
        string description,
        int? timerDurationInSeconds,
        CancellationToken cancellationToken)
    {
        RecipeStep entity = new()
        {
            RecipeId = recipeId,
            Description = description,
            Position = position,
            TimerDurationInSeconds = timerDurationInSeconds
        };

        // TODO: validator

        RecipeStep recipeStep = await recipesRepository.CreateRecipeStepAsync(entity,
            cancellationToken);
        return recipeStep;
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
