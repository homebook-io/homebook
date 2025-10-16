using HomeBook.Frontend.Module.Finances.Models;

namespace HomeBook.Frontend.Module.Finances.Contracts;

/// <summary>
///
/// </summary>
public interface ISavingGoalService
{
    /// <summary>
    /// gets all saving goals
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<SavingGoal>> GetAllSavingGoalsAsync(CancellationToken cancellationToken = default);
}
