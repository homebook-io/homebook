using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.Module.Kitchen.Models;

namespace HomeBook.Backend.Module.Kitchen.Contracts;

public interface IRecipesProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="searchFilter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RecipeDto[]> GetRecipesAsync(string searchFilter,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RecipeDto?> GetRecipeByIdAsync(Guid id,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userId"></param>
    /// <param name="description"></param>
    /// <param name="servings"></param>
    /// <param name="durationWorkingMinutes"></param>
    /// <param name="durationCookingMinutes"></param>
    /// <param name="durationRestingMinutes"></param>
    /// <param name="caloriesKcal"></param>
    /// <param name="comments"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateAsync(string name,
        Guid userId,
        string? description,
        int? servings,
        int? durationWorkingMinutes,
        int? durationCookingMinutes,
        int? durationRestingMinutes,
        int? caloriesKcal,
        string? comments,
        string? source,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipeId"></param>
    /// <param name="name"></param>
    /// <param name="quantity"></param>
    /// <param name="unit"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Recipe2RecipeIngredient> AddIngredientToRecipeAsync(Guid recipeId,
        string name,
        double? quantity,
        string? unit,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RecipeIngredient?> GetIngredientByNameAsync(string name,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateIngredientAsync(string name,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipeId"></param>
    /// <param name="position"></param>
    /// <param name="description"></param>
    /// <param name="timerDurationInSeconds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RecipeStep> AddStepToRecipeAsync(Guid recipeId,
        int position,
        string description,
        int? timerDurationInSeconds,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid id,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateNameAsync(Guid id,
        string name,
        CancellationToken cancellationToken);
}
