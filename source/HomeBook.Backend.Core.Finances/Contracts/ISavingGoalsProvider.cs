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

    /// <summary>
    /// /
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="savingGoalId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SavingGoalDto?> GetSavingGoalByIdAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="savingGoal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateSavingGoalAsync(Guid userId,
        string name,
        string color,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyPayment,
        DateTime? targetDate,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="savingGoal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateSavingGoalAsync(Guid userId,
        Guid savingGoalId,
        string name,
        string color,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyPayment,
        DateTime? targetDate,
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
