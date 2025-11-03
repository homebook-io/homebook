using HomeBook.Backend.DTOs.Enums;

namespace HomeBook.Backend.DTOs.Requests.Finances;

public record UpdateSavingGoalAmountsRequest(
    decimal? TargetAmount,
    decimal? CurrentAmount,
    decimal? MonthlyPayment,
    InterestRateOptions? InterestRateOption,
    decimal? InterestRate);
