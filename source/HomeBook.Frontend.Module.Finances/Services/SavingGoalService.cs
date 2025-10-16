using HomeBook.Frontend.Module.Finances.Contracts;
using HomeBook.Frontend.Module.Finances.Models;

namespace HomeBook.Frontend.Module.Finances.Services;

/// <inheritdoc />
public class SavingGoalService : ISavingGoalService
{
    /// <inheritdoc />
    public async Task<IEnumerable<SavingGoal>> GetAllSavingGoalsAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var result = new List<SavingGoal>
        {
            new(Guid.NewGuid(), "New Car", "#ff0000", 20_000, 6_550, DateTime.Now.AddMonths(12)),
            new(Guid.NewGuid(), "Vacation", "#00ff00", 5_000, 2_315, DateTime.Now.AddMonths(6)),
            new(Guid.NewGuid(), "Emergency Fund", "#8888ff", 10_000, 8_765, DateTime.Now.AddMonths(24))
        }.OrderByDescending(x => x.Percentage);

        return result;
    }
}
