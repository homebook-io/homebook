namespace HomeBook.Backend.DTOs.Responses.Finances;

public record CalculatedSavingResponse(short MonthsNeeded,
    decimal MonthlyPayment,
    decimal[] Amounts,
    decimal[] Interests);
