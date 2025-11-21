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
    Task<IEnumerable<RecipeDto>> GetRecipesAsync(string filter,
        CancellationToken cancellationToken = default);

    Task CreateRecipeAsync(string name,
        CancellationToken cancellationToken = default);
}
