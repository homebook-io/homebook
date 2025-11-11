using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Data.Contracts;

public interface IRecipesRepository
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="searchFilter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<Recipe>> GetAsync(string? searchFilter,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="appDbContext"></param>
    /// <returns></returns>
    Task<Recipe?> GetByIdAsync(Guid entityId,
        CancellationToken cancellationToken,
        AppDbContext? appDbContext = null);

    /// <summary>
    ///
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateOrUpdateAsync(Recipe entity,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid entityId,
        CancellationToken cancellationToken);
}
