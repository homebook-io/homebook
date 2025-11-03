namespace HomeBook.Backend.Core.Finances.Models;

public record SavingCalculationResult(short MonthsNeeded,
    decimal MonthlyPayment,
    decimal[] Amounts,
    decimal[] Interests);
