namespace HomeBook.Backend.Requests;

public record SavingGoalRequest(
    string Name,
    string Color,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal MonthlyPayment,
    DateTime? TargetDate);
