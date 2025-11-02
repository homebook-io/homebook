using HomeBook.Backend.DTOs.Enums;

namespace HomeBook.Backend.DTOs.Requests.Finances;

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
