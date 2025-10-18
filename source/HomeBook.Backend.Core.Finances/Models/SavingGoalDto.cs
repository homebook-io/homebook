namespace HomeBook.Backend.Core.Finances.Models;

public record SavingGoalDto(
    Guid Id,
    string Name,
    string Color,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime? TargetDate);
