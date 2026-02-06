using HomeBook.Backend.Module.Finances.Enums;

namespace HomeBook.Backend.Module.Finances.Requests;

public record UpdateSavingGoalAmountsRequest(
    decimal? TargetAmount,
    decimal? CurrentAmount,
    decimal? MonthlyPayment,
    InterestRateOptions? InterestRateOption,
    decimal? InterestRate);
