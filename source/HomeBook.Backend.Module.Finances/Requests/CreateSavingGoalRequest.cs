using HomeBook.Backend.Module.Finances.Enums;

namespace HomeBook.Backend.Module.Finances.Requests;

public record CreateSavingGoalRequest(
    string Name,
    string Color,
    string Icon,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal MonthlyPayment,
    InterestRateOptions? InterestRateOption,
    decimal? InterestRate,
    DateTime? TargetDate);
