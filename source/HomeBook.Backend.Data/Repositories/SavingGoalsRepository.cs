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
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        List<SavingGoal> savingGoals = await dbContext.Set<SavingGoal>()
            .Where(sg => sg.UserId == userId)
            .ToListAsync(cancellationToken);

        return savingGoals;
    }

    public async Task<SavingGoal?> GetSavingGoalByIdAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        SavingGoal? savingGoal = await dbContext.Set<SavingGoal>()
            .Where(sg => sg.UserId == userId
                         && sg.Id == savingGoalId)
            .FirstOrDefaultAsync(cancellationToken);

        return savingGoal;
    }

    public async Task<Guid> CreateOrUpdateSavingGoalAsync(Guid userId,
        SavingGoal entity,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        var existing = await dbContext.SavingGoals
            .FirstOrDefaultAsync(x => x.Id == entity.Id && x.UserId == userId, cancellationToken);

        if (existing is null)
        {
            // Neues Ziel hinzufügen
            entity.UserId = userId;
            dbContext.SavingGoals.Add(entity);
        }
        else
        {
            // Bestehendes Ziel aktualisieren
            dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task DeleteSavingGoalAsync(Guid userId,
        Guid savingGoalId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<SavingGoal>()
            .Where(sg => sg.UserId == userId && sg.Id == savingGoalId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
