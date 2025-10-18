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
}
