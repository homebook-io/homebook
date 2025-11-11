using HomeBook.Backend.Data.Contracts;
using HomeBook.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeBook.Backend.Data.Repositories;

/// <inheritdoc />
public class SavingGoalsRepository(IDbContextFactory<AppDbContext> factory)
    : ISavingGoalsRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<SavingGoal>> GetAllSavingGoalsAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        List<SavingGoal> entities = await dbContext.Set<SavingGoal>()
            .Where(sg => sg.UserId == userId)
            .ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<SavingGoal?> GetByIdAsync(Guid userId,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        SavingGoal? entity = await dbContext.Set<SavingGoal>()
            .Where(e => e.UserId == userId
                        && e.Id == entityId)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }

    public async Task<Guid> CreateOrUpdateAsync(Guid userId,
        SavingGoal entity,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        SavingGoal? existing = await dbContext.SavingGoals
            .FirstOrDefaultAsync(x => x.Id == entity.Id
                                      && x.UserId == userId,
                cancellationToken);

        if (existing is null)
        {
            entity.UserId = userId;
            dbContext.SavingGoals.Add(entity);
        }
        else
        {
            dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task DeleteAsync(Guid userId,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        await using AppDbContext dbContext = await factory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<SavingGoal>()
            .Where(e => e.UserId == userId
                        && e.Id == entityId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
