using System.Diagnostics;
using HomeBook.Backend.DTOs.Enums;

namespace HomeBook.Backend.DTOs.Responses.Finances;

[DebuggerDisplay("{Name} ({CurrentAmount} / {TargetAmount})")]
public record SavingGoalResponse(
    Guid Id,
    string Name,
    string Color,
    string Icon,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal MonthlyPayment,
    InterestRateOptions InterestRateOption,
    decimal? InterestRate,
    DateTime? TargetDate);
