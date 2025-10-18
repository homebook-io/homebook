using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.Responses;

namespace HomeBook.Backend.Mappings;

public static class SavingGoalMappings
{
    public static FinanceSavingGoalResponse ToResponse(this SavingGoalDto savingGoal)
    {
        return new FinanceSavingGoalResponse(
            savingGoal.Id,
            savingGoal.Name,
            savingGoal.Color,
            savingGoal.TargetAmount,
            savingGoal.CurrentAmount,
            savingGoal.TargetDate);
    }
}
