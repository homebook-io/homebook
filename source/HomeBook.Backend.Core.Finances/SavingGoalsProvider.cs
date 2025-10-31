using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Mappings;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Core.Finances;

public class SavingGoalsProvider(
    ILogger<SavingGoalsProvider> logger,
    ISavingGoalsRepository savingGoalsRepository) : ISavingGoalsProvider
{
    /// <inheritdoc />
    public async Task<SavingGoalDto[]> GetAllSavingGoalsAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving all saving goals for user. UserId: {UserId}",
            userId);

        IEnumerable<SavingGoal> savingGoalEntities = await savingGoalsRepository.GetAllSavingGoalsAsync(userId,
            cancellationToken);
        SavingGoalDto[] savingGoals = savingGoalEntities
            .Select(sg => sg.ToDto())
            .ToArray();

        return savingGoals;
    }

    /// <inheritdoc />
    public async Task<SavingGoalDto?> GetSavingGoalByIdAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken) =>
        (await savingGoalsRepository.GetSavingGoalByIdAsync(userId,
            savingGoalId,
            cancellationToken))?.ToDto();

    /// <inheritdoc />
    public async Task<Guid> CreateSavingGoalAsync(Guid userId,
        string name,
        string color,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyPayment,
        DateTime? targetDate,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = new()
        {
            UserId = userId,
            Name = name,
            Color = color,
            TargetAmount = targetAmount,
            MonthlyPayment = monthlyPayment,
            CurrentAmount = currentAmount,
            TargetDate = targetDate
        };

        // TODO: validator

        Guid savingGoalId = await savingGoalsRepository.CreateOrUpdateSavingGoalAsync(userId, entity, cancellationToken);
        return savingGoalId;
    }

    /// <inheritdoc />
    public async Task UpdateSavingGoalAsync(Guid userId,
        Guid savingGoalId,
        string name,
        string color,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyPayment,
        DateTime? targetDate,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = new()
        {
            Id = savingGoalId,
            UserId = userId,
            Name = name,
            Color = color,
            TargetAmount = targetAmount,
            MonthlyPayment = monthlyPayment,
            CurrentAmount = currentAmount,
            TargetDate = targetDate
        };

        // TODO: validator

        await savingGoalsRepository.CreateOrUpdateSavingGoalAsync(userId, entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteSavingGoalAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken) =>
        await savingGoalsRepository.DeleteSavingGoalAsync(userId,
            savingGoalId,
            cancellationToken);
}
