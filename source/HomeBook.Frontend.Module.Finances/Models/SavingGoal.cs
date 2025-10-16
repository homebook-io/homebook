namespace HomeBook.Frontend.Module.Finances.Models;

public record SavingGoal(
    Guid Id,
    string Name,
    string Color,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime TargetDate)
{
    public double Percentage => TargetAmount == 0 ? 0 : (double)(CurrentAmount / TargetAmount);
}
