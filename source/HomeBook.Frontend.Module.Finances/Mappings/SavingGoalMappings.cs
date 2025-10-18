using HomeBook.Frontend.Module.Finances.Models;

namespace HomeBook.Frontend.Module.Finances.Mappings;

public static class SavingGoalMappings
{
    public static SavingGoalDto ToDto(this HomeBook.Client.Models.FinanceSavingGoalResponse savingGoal)
    {
        return new SavingGoalDto(
            savingGoal.Id!.Value,
            savingGoal.Name!,
            savingGoal.Color!,
            Convert.ToDecimal(savingGoal.TargetAmount),
            Convert.ToDecimal(savingGoal.CurrentAmount),
            savingGoal.TargetDate?.UtcDateTime ?? null);
    }
}
