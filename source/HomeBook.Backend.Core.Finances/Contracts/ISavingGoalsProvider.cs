using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.DTOs.Enums;

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
    /// <param name="name"></param>
    /// <param name="color"></param>
    /// <param name="icon"></param>
    /// <param name="targetAmount"></param>
    /// <param name="currentAmount"></param>
    /// <param name="monthlyPayment"></param>
    /// <param name="interestRateOption"></param>
    /// <param name="interestRate"></param>
    /// <param name="targetDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid> CreateSavingGoalAsync(Guid userId,
        string name,
        string color,
        string icon,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyPayment,
        InterestRateOptions? interestRateOption,
        decimal? interestRate,
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
