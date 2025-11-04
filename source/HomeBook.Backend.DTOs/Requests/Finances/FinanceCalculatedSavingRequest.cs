using HomeBook.Backend.DTOs.Enums;

namespace HomeBook.Backend.DTOs.Requests.Finances;

/// <summary>
///
/// </summary>
/// <param name="TargetAmount"></param>
/// <param name="TargetDate"></param>
/// <param name="InterestRateOption"></param>
/// <param name="InterestRate"></param>
/// <param name="TargetSimpleRate"></param>
public record FinanceCalculatedSavingRequest(
    decimal TargetAmount,
    DateTime TargetDate,
    InterestRateOptions InterestRateOption,
    decimal InterestRate,
    bool TargetSimpleRate);
