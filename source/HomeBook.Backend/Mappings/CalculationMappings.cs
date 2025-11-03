using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.DTOs.Responses.Finances;

namespace HomeBook.Backend.Mappings;

public static class CalculationMappings
{
    public static FinanceCalculatedSavingResponse ToResponse(this SavingCalculationResult r) =>
        new(r.MonthsNeeded,
            r.MonthlyPayment,
            r.Amounts,
            r.Interests);
}
