using HomeBook.Backend.Module.Finances.Enums;

namespace HomeBook.Backend.Module.Finances.Requests;

/// <summary>
///
/// </summary>
/// <param name="TargetAmount"></param>
/// <param name="TargetDate"></param>
/// <param name="InterestRateOption"></param>
/// <param name="InterestRate"></param>
/// <param name="TargetSimpleRate"></param>
public record CalculateSavingRequest(
    decimal TargetAmount,
    DateTime TargetDate,
    InterestRateOptions InterestRateOption,
    decimal InterestRate,
    bool TargetSimpleRate);
