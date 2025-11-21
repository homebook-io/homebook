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
    /// <param name="description"></param>
    /// <param name="duration"></param>
    /// <param name="caloriesKcal"></param>
    /// <param name="servings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateAsync(string name,
        string? description,
        TimeSpan? duration,
        int? caloriesKcal,
        int? servings,
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
