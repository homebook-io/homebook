using HomeBook.Frontend.Module.Kitchen.Models;

namespace HomeBook.Frontend.Module.Kitchen.Contracts;

public interface IMealService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<RecipeDto>> GetMealsAsync(string filter,
        CancellationToken cancellationToken = default);
}
