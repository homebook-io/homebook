using System.Diagnostics;

namespace HomeBook.Backend.Responses;

[DebuggerDisplay("{Name} ({CurrentAmount} / {TargetAmount})")]
public record FinanceSavingGoalResponse(
    Guid Id,
    string Name,
    string Color,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime? TargetDate);
