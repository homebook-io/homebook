using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class SavingGoalsRepository(
    ILogger<SavingGoalsRepository> logger,
    IDbContextFactory<AppDbContext> factory) : ISavingGoalsRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<SavingGoal>> GetAllSavingGoalsAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        // await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);
        //
        // List<SavingGoal> savingGoals = await dbContext.Set<SavingGoal>()
        //     .Where(sg => sg.UserId == userId)
        //     .ToListAsync(cancellationToken);

        List<SavingGoal> savingGoals = [];
        savingGoals.Add(new SavingGoal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Color = "#FF5733",
            Name = "New House",
            TargetAmount = 50_000,
            CurrentAmount = 15_000,
            TargetDate = DateTime.UtcNow.AddMonths(12)
        });
        savingGoals.Add(new SavingGoal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Color = "#00ff85",
            Name = "New Car",
            TargetAmount = 12_500,
            CurrentAmount = 10_855
        });

        return savingGoals;
    }
}
