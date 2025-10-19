using HomeBook.Backend.Data.Entities;

namespace HomeBook.Backend.Data.Contracts;

/// <summary>
///
/// </summary>
public interface ISavingGoalsRepository
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<SavingGoal>> GetAllSavingGoalsAsync(Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// /
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="savingGoalId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SavingGoal?> GetSavingGoalByIdAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateOrUpdateSavingGoalAsync(Guid userId,
        SavingGoal entity,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="savingGoalId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteSavingGoalAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken);
}
