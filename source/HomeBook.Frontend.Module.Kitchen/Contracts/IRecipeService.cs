using HomeBook.Frontend.Module.Kitchen.Models;

namespace HomeBook.Frontend.Module.Kitchen.Contracts;

public interface IRecipeService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<RecipeDto>> GetRecipesAsync(string? filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RecipeDetailDto?> GetRecipeByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="durationInMinutes"></param>
    /// <param name="caloriesKcal"></param>
    /// <param name="servings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateRecipeAsync(string name,
        string? description = null,
        int? durationInMinutes = null,
        int? caloriesKcal = null,
        int? servings = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateRecipeAsync(string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///
    /// </summary>
    /// <param name="recipeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteRecipeAsync(Guid recipeId,
        CancellationToken cancellationToken = default);
}
