using HomeBook.Backend.Core.Finances.Models;

namespace HomeBook.Backend.Core.Finances.Mappings;

public static class SavingGoalMappings
{
    public static SavingGoalDto ToDto(this Data.Entities.SavingGoal savingGoal)
    {
        return new SavingGoalDto(
            savingGoal.Id,
            savingGoal.Name,
            savingGoal.Color,
            savingGoal.Icon ?? string.Empty,
            savingGoal.TargetAmount,
            savingGoal.CurrentAmount,
            savingGoal.MonthlyPayment,
            (DTOs.Enums.InterestRateOptions)savingGoal.InterestRateOption,
            savingGoal.InterestRate,
            savingGoal.TargetDate);
    }
}
