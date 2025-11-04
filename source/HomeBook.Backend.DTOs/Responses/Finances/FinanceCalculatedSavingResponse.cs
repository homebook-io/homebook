namespace HomeBook.Backend.DTOs.Responses.Finances;

public record FinanceCalculatedSavingResponse(short MonthsNeeded,
    decimal MonthlyPayment,
    decimal[] Amounts,
    decimal[] Interests);
