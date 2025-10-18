namespace HomeBook.Backend.Responses;

public record FinanceSavingGoalResponse(
    Guid Id,
    string Name,
    string Color,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime? TargetDate);
