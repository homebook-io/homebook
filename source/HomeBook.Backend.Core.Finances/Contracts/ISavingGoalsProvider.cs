using HomeBook.Backend.Core.Finances.Models;

namespace HomeBook.Backend.Core.Finances.Contracts;

/// <summary>
///
/// </summary>
public interface ISavingGoalsProvider
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SavingGoalDto[]> GetAllSavingGoalsAsync(Guid userId,
        CancellationToken cancellationToken);
}
