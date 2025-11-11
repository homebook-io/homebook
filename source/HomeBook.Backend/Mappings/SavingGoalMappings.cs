using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.DTOs.Responses.Finances;
using HomeBook.Backend.Responses;

namespace HomeBook.Backend.Mappings;

public static class SavingGoalMappings
{
    public static SavingGoalResponse ToResponse(this SavingGoalDto savingGoal)
    {
        return new SavingGoalResponse(
            savingGoal.Id,
            savingGoal.Name,
            savingGoal.Color,
            savingGoal.Icon,
            savingGoal.TargetAmount,
            savingGoal.CurrentAmount,
            savingGoal.MonthlyPayment,
            savingGoal.InterestRateOption,
            savingGoal.InterestRate,
            savingGoal.TargetDate);
    }
}
