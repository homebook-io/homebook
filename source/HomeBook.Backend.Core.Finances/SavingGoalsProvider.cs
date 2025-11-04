using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Mappings;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using HomeBook.Backend.DTOs.Enums;
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
        string icon,
        decimal targetAmount,
        decimal currentAmount,
        decimal monthlyPayment,
        InterestRateOptions? interestRateOption,
        decimal? interestRate,
        DateTime? targetDate,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = new()
        {
            UserId = userId,
            Name = name,
            Color = color,
            Icon = icon,
            TargetAmount = targetAmount,
            CurrentAmount = currentAmount,
            MonthlyPayment = monthlyPayment,
            InterestRateOption = (SavingGoal.InterestRateOptions)interestRateOption!,
            InterestRate = interestRate,
            TargetDate = targetDate?.ToUniversalTime().Date ?? null
        };

        // TODO: validator

        Guid savingGoalId = await savingGoalsRepository
            .CreateOrUpdateSavingGoalAsync(userId,
                entity,
                cancellationToken);
        return savingGoalId;
    }

    /// <inheritdoc />
    public async Task UpdateSavingGoalNameAsync(Guid userId,
        Guid savingGoalId,
        string name,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = await savingGoalsRepository.GetSavingGoalByIdAsync(userId,
                                savingGoalId,
                                cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Saving goal with ID {savingGoalId} not found for user {userId}.");

        entity.Name = name;

        await savingGoalsRepository.CreateOrUpdateSavingGoalAsync(userId,
            entity,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateSavingGoalAppearanceAsync(Guid userId,
        Guid savingGoalId,
        string color,
        string icon,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = await savingGoalsRepository.GetSavingGoalByIdAsync(userId,
                                savingGoalId,
                                cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Saving goal with ID {savingGoalId} not found for user {userId}.");

        entity.Color = color;
        entity.Icon = icon;

        await savingGoalsRepository.CreateOrUpdateSavingGoalAsync(userId,
            entity,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateSavingGoalAmountsAsync(Guid userId,
        Guid savingGoalId,
        decimal? targetAmount,
        decimal? currentAmount,
        decimal? monthlyPayment,
        InterestRateOptions? interestRateOption,
        decimal? interestRate,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = await savingGoalsRepository.GetSavingGoalByIdAsync(userId,
                                savingGoalId,
                                cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Saving goal with ID {savingGoalId} not found for user {userId}.");

        if (targetAmount.HasValue)
            entity.TargetAmount = targetAmount.Value;
        if (currentAmount.HasValue)
            entity.CurrentAmount = currentAmount.Value;
        if (monthlyPayment.HasValue)
            entity.MonthlyPayment = monthlyPayment.Value;
        if (interestRateOption.HasValue)
            entity.InterestRateOption = (SavingGoal.InterestRateOptions)interestRateOption.Value;
        if (interestRate.HasValue)
            entity.InterestRate = interestRate.Value;

        await savingGoalsRepository.CreateOrUpdateSavingGoalAsync(userId,
            entity,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateSavingGoalInfoAsync(Guid userId,
        Guid savingGoalId,
        DateTime? targetDate,
        CancellationToken cancellationToken)
    {
        SavingGoal entity = await savingGoalsRepository.GetSavingGoalByIdAsync(userId,
                                savingGoalId,
                                cancellationToken)
                            ?? throw new KeyNotFoundException(
                                $"Saving goal with ID {savingGoalId} not found for user {userId}.");

        if (targetDate.HasValue)
            entity.TargetDate = targetDate.Value;

        await savingGoalsRepository.CreateOrUpdateSavingGoalAsync(userId,
            entity,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteSavingGoalAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken) =>
        await savingGoalsRepository.DeleteSavingGoalAsync(userId,
            savingGoalId,
            cancellationToken);
}
